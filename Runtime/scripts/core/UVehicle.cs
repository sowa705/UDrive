using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UVehicle : MonoBehaviour
{
    ITorqueGenerator[] TqGenerators;
    UWheelCollider[] UWheelColliders;
    Rigidbody rigidbody;
    public Rigidbody Rigidbody { get => rigidbody; }

    [Range(1, 32)]
    public int Substeps = 4;
    public float CurrentDeltaT { get => Time.fixedDeltaTime / Substeps; }

    Dictionary<VehicleParamId, float> VehicleValues;
    Dictionary<VehicleParamId, float> InputParameters;

    List<VehicleComponent> Components = new List<VehicleComponent>();
    Dictionary<int,IStatefulComponent> StatefulComponents=new Dictionary<int, IStatefulComponent>();
    void Awake()
    {
        ResetVehicle();
    }
    public float ReadParameter(VehicleParamId paramID)
    {
        if (VehicleValues.ContainsKey(paramID))
        {
            return VehicleValues[paramID];
        }
        return 0;
    }
    public void WriteParameter(VehicleParamId paramID,float value)
    {
        if (VehicleValues.ContainsKey(paramID))
        {
            VehicleValues[paramID]=value;
            return;
        }
        VehicleValues.Add(paramID,value);
    }

    public float ReadInputParameter(VehicleParamId paramID)
    {
        if (InputParameters.ContainsKey(paramID))
        {
            return InputParameters[paramID];
        }
        return 0;
    }
    public void WriteInputParameter(VehicleParamId paramID, float value)
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
        //get a somewhat stable and consistent ID
        int cid = component.transform.localPosition.GetHashCode()^component.name.GetHashCode()^component.GetType().Name.GetHashCode();

        Debug.Log($"Added component {component.name}, {cid}");
        component.ID = cid;
        Components.Add(component);

        if (component is IStatefulComponent)
        {
            StatefulComponents.Add(cid,component as IStatefulComponent);
        }
    }
    public UWheelCollider[] GetWheels()
    {
        return UWheelColliders;
    }
    void ResetVehicle()
    {
        TqGenerators=GetComponentsInChildren<ITorqueGenerator>();
        UWheelColliders=GetComponentsInChildren<UWheelCollider>();
        rigidbody = GetComponentInChildren<Rigidbody>();
        VehicleValues = new Dictionary<VehicleParamId, float>();
        InputParameters = new Dictionary<VehicleParamId, float>();
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
        writer.Write(rigidbody.position.x);
        writer.Write(rigidbody.position.y);
        writer.Write(rigidbody.position.z);
        writer.Write(rigidbody.velocity.x);
        writer.Write(rigidbody.velocity.y);
        writer.Write(rigidbody.velocity.z);
        writer.Write(rigidbody.rotation.x);
        writer.Write(rigidbody.rotation.y);
        writer.Write(rigidbody.rotation.z);
        writer.Write(rigidbody.rotation.w);
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
        int m = reader.ReadInt32();
        if (m==0)
        {
            throw new System.Exception("Invalid start header");
        }
        SerializationMode mode = (SerializationMode)m;
        Vector3 pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        Vector3 vel = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        Quaternion rot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        Debug.Log($"Position diff = {pos-rigidbody.position}");
        Debug.Log($"Velocity diff = {Vector3.Distance(vel, rigidbody.velocity) * 1000}");
        Debug.Log($"Rotation diff = {Quaternion.Angle(rot,rigidbody.rotation) * 1000}");

        while (true)
        {
            VehicleParamId key = (VehicleParamId)reader.ReadInt32();
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
        WriteParameter(VehicleParamId.VehicleSpeed,Rigidbody.velocity.magnitude);
        for (int i = 0; i < Substeps; i++)
        {
            RunSubstep();
        }
    }
}

public enum VehicleParamId
{
    VehicleEnabled,
    EngineEnabled,
    EngineRPM,
    EngineMaxRPM,
    VehicleSpeed,
    SteeringInput,
    AcceleratorInput,
    BrakeInput,
    CurrentGear
}

public enum SerializationMode
{
    None,
    Network,
    Full
}