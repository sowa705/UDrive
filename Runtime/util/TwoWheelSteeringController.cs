using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWheelSteeringController : MonoBehaviour
{
    public UWheelCollider LeftWheel;
    public UWheelCollider RightWheel;
    public SteeringMode Mode;

    [Range(-1f, 1f)]
    public float SteerInput = 0;
    [Range(0f, 45f)]
    public float MaxSteeringAngle = 25;

    float WheelBase;
    float Width;
    private void Start()
    {
        SetupWheelGeometry();
    }
    void SetupWheelGeometry()
    {
        var wheels=GetComponentInParent<UVehicle>().GetWheels();
        Vector4 dimensions = Vector4.zero;
        foreach (var item in wheels)
        {
            var pos = item.transform.localPosition;
            if (dimensions.x>pos.x)
            {
                dimensions.x = pos.x;
            }
            if (dimensions.y > pos.x)
            {
                dimensions.y = pos.x;
            }
            if (dimensions.z > pos.z)
            {
                dimensions.z = pos.z;
            }
            if (dimensions.w > pos.z)
            {
                dimensions.w = pos.z;
            }
        }
        Width = dimensions.x - dimensions.y;
        WheelBase = dimensions.z - dimensions.w;
    }
    void FixedUpdate()
    {
        switch (Mode)
        {
            case SteeringMode.Simple:
                LeftWheel.SteerAngle = SteerInput * MaxSteeringAngle;
                RightWheel.SteerAngle = SteerInput * MaxSteeringAngle;
                break;
        }
    }
}

public enum SteeringMode
{
    Simple,
    Ackermann,
    InverseAckermann
}