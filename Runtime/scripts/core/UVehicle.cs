using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UDrive;
using UnityEngine;

namespace UDrive
{
    /// <summary>
    /// Core of a UDrive Vehicle
    /// </summary>
    public class UVehicle : MonoBehaviour
    {
        ITorqueGenerator[] TqGenerators;
        UWheelCollider[] UWheelColliders;
        Rigidbody rb;
        public Rigidbody Rigidbody { get => rb; }

        public Transform CenterOfMass;

        [Range(1, 32)]
        public int Substeps = 4;
        public float SubstepDeltaT { get => Time.fixedDeltaTime / Substeps; }

        public Dictionary<VehicleParameter, float> VehicleValues { get; private set; }
        public Dictionary<VehicleInputParameter, float> InputParameters { get; private set; }

        List<VehicleComponent> Components = new List<VehicleComponent>();
        public Dictionary<int, IStatefulComponent> StatefulComponents { get; } = new Dictionary<int, IStatefulComponent>();
        public List<IDebuggableComponent> DebuggableComponents { get; } = new List<IDebuggableComponent>();
        List<IVehicleAssist> VehicleAssists = new List<IVehicleAssist>();
        Vector3 lastVelocity;

        public VehicleDeserializer Deserializer { get=> deserializer; set { if (value != null) value.SetVehicle(this); deserializer = value; } }
        VehicleDeserializer deserializer;

        public VehicleSerializer Serializer { get => serializer; set { if(value!=null) value.SetVehicle(this); serializer = value; } }
        VehicleSerializer serializer;

        void Awake()
        {
            ResetVehicle();
        }
        public float ReadParameter(VehicleParameter paramID)
        {
            if (VehicleValues.ContainsKey(paramID))
            {
                return VehicleValues[paramID];
            }
            return 0;
        }
        public void WriteParameter(VehicleParameter paramID, float value)
        {
            if (VehicleValues.ContainsKey(paramID))
            {
                VehicleValues[paramID] = value;
                return;
            }
            VehicleValues.Add(paramID, value);
        }

        public float ReadInputParameter(VehicleInputParameter paramID)
        {
            if (InputParameters.ContainsKey(paramID))
            {
                return InputParameters[paramID];
            }
            return 0;
        }
        public void WriteInputParameter(VehicleInputParameter paramID, float value)
        {
            if (InputParameters.ContainsKey(paramID))
            {
                InputParameters[paramID] = value;
                return;
            }
            InputParameters.Add(paramID, value);
        }

        public void AddVehicleComponent(VehicleComponent component)
        {
            //Debug.Log($"Added component {component.name}, {component.GetID()}");
            Components.Add(component);

            if (component is IStatefulComponent)
            {
                StatefulComponents.Add(component.GetID(), component as IStatefulComponent);
            }
            if (component is IDebuggableComponent)
            {
                DebuggableComponents.Add(component as IDebuggableComponent);
            }
            if (component is IVehicleAssist)
            {
                VehicleAssists.Add(component as IVehicleAssist);
            }
        }

        public UWheelCollider[] GetWheels()
        {
            return UWheelColliders;
        }
        public void ResetVehicle()
        {
            TqGenerators = GetComponentsInChildren<ITorqueGenerator>();
            UWheelColliders = GetComponentsInChildren<UWheelCollider>();
            rb = GetComponentInChildren<Rigidbody>();

            rb.ResetInertiaTensor();
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            VehicleValues = new Dictionary<VehicleParameter, float>();
            InputParameters = new Dictionary<VehicleInputParameter, float>();

            foreach (var item in UWheelColliders)
            {
                item.ResetWheel();
            }
            foreach (var item in Components)
            {
                item.VehicleStart();
            }

            if (CenterOfMass != null)
            {
                rb.centerOfMass = CenterOfMass.localPosition;
            }
        }

        void RunSubstep()
        {
            //Powertrain update
            foreach (var item in TqGenerators)
            {
                item.RunSubstep();
            }
            foreach (var item in VehicleAssists)
            {
                foreach (var wheel in UWheelColliders)
                {
                    item.OnWheel(wheel);
                }
            }
            foreach (var item in UWheelColliders)
            {
                item.RunSubstep(SubstepDeltaT);
            }
            foreach (var item in Components)
            {
                item.Substep();
            }
        }

        private void FixedUpdate()
        {
            if (deserializer!=null)
            {
                deserializer.RunUpdate();
            }

            Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
            var localAcceleration = transform.InverseTransformVector(acceleration);

            WriteParameter(VehicleParameter.VehicleLongitudinalAcceleration, localAcceleration.z);
            WriteParameter(VehicleParameter.VehicleLateralAcceleration, localAcceleration.x);
            int layermask = Physics.DefaultRaycastLayers & ~(LayerMask.GetMask("IgnoreVehicleRaycast"));
            RaycastHit hit;
            Ray r = new Ray(transform.position + transform.up, -transform.up);
            Debug.DrawRay(r.origin, r.direction * 3, Color.cyan);
            if (Physics.Raycast(r, out hit, 3f, layermask))
            {
                WriteParameter(VehicleParameter.RoadGrade, Vector3.Angle(hit.normal, Vector3.up) * 2);
            }


            lastVelocity = rb.velocity;
            WriteParameter(VehicleParameter.VehicleSpeed, Rigidbody.velocity.magnitude);
            foreach (var item in VehicleAssists)
            {
                item.OnUpdate();
            }

            for (int i = 0; i < Substeps; i++)
            {
                RunSubstep();
            }

            if (serializer != null)
            {
                serializer.RunUpdate();
            }
        }
    }
}