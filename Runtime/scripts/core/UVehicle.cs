using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UVehicle : MonoBehaviour
{
    ITorqueGenerator[] TqGenerators;
    UWheelCollider[] UWheelColliders;
    Rigidbody rb;
    public Rigidbody Rigidbody { get => rb; }

    public Transform CenterOfMass;

    [Range(1, 32)]
    public int Substeps = 4;
    public float CurrentDeltaT { get => Time.fixedDeltaTime / Substeps; }

    public Dictionary<VehicleParameter, float> VehicleValues { get; private set; }
    public Dictionary<VehicleInputParameter, float> InputParameters { get; private set; }

    List<VehicleComponent> Components = new List<VehicleComponent>();
    Dictionary<int,IStatefulComponent> StatefulComponents=new Dictionary<int, IStatefulComponent>();
    public List<IDebuggableComponent> DebuggableComponents { get; } = new List<IDebuggableComponent>();

    Vector3 lastVelocity;
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
    public void WriteParameter(VehicleParameter paramID,float value)
    {
        if (VehicleValues.ContainsKey(paramID))
        {
            VehicleValues[paramID]=value;
            return;
        }
        VehicleValues.Add(paramID,value);
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
        Debug.Log($"Added component {component.name}, {component.GetID()}");
        Components.Add(component);

        if (component is IStatefulComponent)
        {
            StatefulComponents.Add(component.GetID(), component as IStatefulComponent);
        }
        if (component is IDebuggableComponent)
        {
            DebuggableComponents.Add(component as IDebuggableComponent);
        }
    }

    public UWheelCollider[] GetWheels()
    {
        return UWheelColliders;
    }
    public void ResetVehicle()
    {
        TqGenerators=GetComponentsInChildren<ITorqueGenerator>();
        UWheelColliders=GetComponentsInChildren<UWheelCollider>();
        rb = GetComponentInChildren<Rigidbody>();

        rb.ResetInertiaTensor();

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

        if (CenterOfMass!=null)
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
        foreach (var item in UWheelColliders)
        {
            item.RunSubstep(CurrentDeltaT);
        }
    }

    public void SerializeState(BinaryWriter writer,SerializationMode mode)
    {
        writer.Write((int)mode);
        if (mode == 0)
        {
            return;
        }
        writer.Write(rb.position.x);
        writer.Write(rb.position.y);
        writer.Write(rb.position.z);
        writer.Write(rb.velocity.x);
        writer.Write(rb.velocity.y);
        writer.Write(rb.velocity.z);
        writer.Write(rb.rotation.x);
        writer.Write(rb.rotation.y);
        writer.Write(rb.rotation.z);
        writer.Write(rb.rotation.w);
        foreach (var item in InputParameters)
        {
            writer.Write((int)item.Key);
            writer.Write(item.Value);
        }
        writer.Write(0);
        if (mode==SerializationMode.Full)
        {
            foreach (var item in StatefulComponents)
            {
                writer.Write(item.Key);
                item.Value.SerializeState(writer);
            }
            writer.Write(0);
        }
    }
    public void DeserializeState(BinaryReader reader)
    {
        int m;
        try
        {
            m = reader.ReadInt32();
        }
        catch (System.Exception)
        {
            throw new System.Exception("Cannot read from the stream");
        }
        if (m==0)
        {
            throw new System.Exception("Invalid start header");
        }
        SerializationMode mode = (SerializationMode)m;
        if (m==0)
        {
            return;
        }
        Vector3 pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        Vector3 vel = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        Quaternion rot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        Debug.Log($"Position diff = {Vector3.Distance(pos, transform.position)}");
        Debug.Log($"Velocity diff = {Vector3.Distance(vel, rb.velocity)}");
        Debug.Log($"Rotation diff = {Quaternion.Angle(rot,rb.rotation)}");
        float dist = Vector3.Distance(pos, transform.position);
        if (dist > .5f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, vel, Time.fixedDeltaTime * dist);
            rb.position = Vector3.Lerp(rb.position, pos, Time.fixedDeltaTime * dist);
            rb.rotation = Quaternion.Slerp(rb.rotation, rot, Time.fixedDeltaTime * dist);
        }

        while (true)
        {
            VehicleInputParameter key = (VehicleInputParameter)reader.ReadInt32();
            if (key == 0)
            {
                break;
            }
            WriteInputParameter(key,reader.ReadSingle());
        }
        if (mode==SerializationMode.Full)
        {
            while (true)
            {
                int key = reader.ReadInt32();
                if (key == 0)
                {
                    break;
                }
                if (!StatefulComponents.ContainsKey(key))
                {
                    throw new System.Exception("Serialized stream contains unknown key");
                }
                StatefulComponents[key].Deserialize(reader);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 acceleration = (rb.velocity - lastVelocity)/Time.fixedDeltaTime;
        var localAcceleration = transform.InverseTransformVector(acceleration);

        WriteParameter(VehicleParameter.VehicleLongitudinalAcceleration,localAcceleration.z);
        WriteParameter(VehicleParameter.VehicleLateralAcceleration, localAcceleration.x);
        int layermask = Physics.DefaultRaycastLayers & ~(LayerMask.GetMask("IgnoreCameraRaycast"));
        RaycastHit hit;
        Ray r = new Ray(transform.position+transform.up, -transform.up);
        Debug.DrawRay(r.origin,r.direction*3,Color.cyan);
        if (Physics.Raycast(r,out hit,3f, layermask))
        {
            WriteParameter(VehicleParameter.RoadGrade, Vector3.Angle(hit.normal,Vector3.up)*2);
        }


        lastVelocity = rb.velocity;
        WriteParameter(VehicleParameter.VehicleSpeed,Rigidbody.velocity.magnitude);
        for (int i = 0; i < Substeps; i++)
        {
            RunSubstep();
        }
    }
}
