using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UDrive
{
    /// <summary>
    /// Wheel collider with integrated suspension, requires UVehicle to function
    /// </summary>
    public class UWheelCollider : VehicleComponent, ITorqueNode, IStatefulComponent
    {
        public WheelParametersObject ParametersObject;
        public WheelParameters Parameters;
        [NonSerialized]
        public WheelState wheelState;
        public WheelTickState LastTickState { get; private set; } = new WheelTickState();
        [NonSerialized]
        public Rigidbody parentRB;

        public float EngineTorque;
        public float BrakeTorque;

        public float LForceMultip;

        public float SteerAngle;

        Vector3 lastworldpos;
        Vector3 worldpos;

        public Vector3 Velocity;
        List<WheelComponent> wheelComponents = new List<WheelComponent>();

        public void ResetWheel()
        {
            if (ParametersObject != null)
            {
                Parameters = ParametersObject.Parameters;
            }
            wheelState = new WheelState();
            foreach (var component in wheelComponents)
            {
                component.OnDetach();
            }
            wheelComponents.Clear();

            wheelComponents.Add(new RigidbodySuspensionComponent(this));
            wheelComponents.Add(new TireFrictionComponent(this));
            wheelComponents.Add(new WheelInertiaComponent(this));

            parentRB = GetComponentInParent<Rigidbody>();
            LastTickState = new WheelTickState();
        }
        public Vector3 GetWheelPosition()
        {
            return transform.position + wheelState.SuspensionPosition * transform.up;
        }
        public Vector3 GetLocalWheelPosition()
        {
            return wheelState.SuspensionPosition * Vector3.up;
        }
        public Quaternion GetWheelColliderLocalRotation()
        {
            if (Parameters.RotateColliderMesh)
            {
                return Quaternion.Euler(wheelState.RotationAngle, SteerAngle, 0);
            }
            else
            {
                return Quaternion.Euler(0, SteerAngle, 0);
            }
        }
        void OnDrawGizmos()
        {
            if (wheelState == null)
            {
                OnValidate();
            }
            Vector3 dir = transform.up;
            Vector3 worldPos = transform.position;
            Vector3 normal = Quaternion.Euler(0, SteerAngle, 0) * transform.right;

            Vector3 rayPos = GetWheelPosition();

            Vector3 rotDir = Quaternion.Euler(wheelState.RotationAngle, SteerAngle, 0) * transform.forward;

            float rayDist = Parameters.Radius;

            Gizmos.color = Color.red;

            Gizmos.DrawLine(rayPos, rayPos - dir * rayDist);

            Gizmos.color = Color.blue;

            Gizmos.DrawLine(rayPos, rayPos - rotDir * rayDist);

#if UNITY_EDITOR
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(
                Parameters.ColliderMesh, 
                rayPos, 
                transform.rotation * GetWheelColliderLocalRotation(), 
                new Vector3(Parameters.Width, Parameters.Radius * 2, Parameters.Radius * 2));
#endif
        }
        private void OnValidate()
        {
            if (ParametersObject != null)
            {
                Parameters = ParametersObject.Parameters;
            }
            wheelState = new WheelState();
        }

        public void RunSubstep(float deltaT)
        {
            LastTickState = new WheelTickState();

            foreach (var item in wheelComponents)
            {
                item.RunSubstep(LastTickState, deltaT);
            }
        }
        void FixedUpdate()
        {
            lastworldpos = worldpos;
            worldpos = transform.position;
            Velocity = (worldpos - lastworldpos) / Time.fixedDeltaTime;
        }
        public float GetRPMFromTorque(float torque)
        {
            EngineTorque = torque;
            return wheelState.AngularVelocity * 9.5493f;
        }

        public override void VehicleStart()
        {
            ResetWheel();
        }

        public void SerializeState(BinaryWriter writer)
        {
            writer.Write(wheelState.SuspensionPosition);
            writer.Write(wheelState.SuspensionVelocity);
            writer.Write(wheelState.AngularVelocity);
        }

        public void Deserialize(BinaryReader reader)
        {
            wheelState.SuspensionPosition = Mathf.Lerp(wheelState.SuspensionPosition, reader.ReadSingle(), Time.fixedDeltaTime);
            wheelState.SuspensionVelocity = Mathf.Lerp(wheelState.SuspensionVelocity, reader.ReadSingle(), Time.fixedDeltaTime);
            wheelState.AngularVelocity = Mathf.Lerp(wheelState.AngularVelocity, reader.ReadSingle(), Time.fixedDeltaTime);
        }
    }
}