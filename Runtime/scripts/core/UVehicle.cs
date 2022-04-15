using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVehicle : MonoBehaviour
{
    ITorqueGenerator[] TqGenerators;
    UWheelCollider[] UWheelColliders;
    Rigidbody Rigidbody;

    [Range(1, 32)]
    public int Substeps = 4;
    public float CurrentDeltaT { get => Time.fixedDeltaTime / Substeps; }

    Dictionary<VehicleParamId, float> VehicleValues;
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
    public UWheelCollider[] GetWheels()
    {
        return UWheelColliders;
    }
    void ResetVehicle()
    {
        TqGenerators=GetComponentsInChildren<ITorqueGenerator>();
        UWheelColliders=GetComponentsInChildren<UWheelCollider>();
        Rigidbody = GetComponentInChildren<Rigidbody>();
        VehicleValues = new Dictionary<VehicleParamId, float>();
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
    TargetSteerAngle,
}