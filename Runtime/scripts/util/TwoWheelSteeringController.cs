using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWheelSteeringController : VehicleComponent
{
    public UWheelCollider LeftWheel;
    public UWheelCollider RightWheel;

    public SteeringMode Mode;

    [Range(-1f, 1f)]
    public float SteerInput = 0;
    [Range(0f, 45f)]
    public float MaxSteeringAngle = 25;
    [Range(10, 500f)]
    public float MaxAngularVelocity=80f;
    [Range(0f, 1f)]
    public float VelocitySomething = 80f;

    float actualSteerAngle = 0;

    float WheelBase;
    float Width;
    public override void VehicleStart()
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
            if (dimensions.y < pos.x)
            {
                dimensions.y = pos.x;
            }
            if (dimensions.z > pos.z)
            {
                dimensions.z = pos.z;
            }
            if (dimensions.w < pos.z)
            {
                dimensions.w = pos.z;
            }
        }
        Width = dimensions.y - dimensions.x;
        WheelBase = dimensions.w - dimensions.z;
    }
    void FixedUpdate()
    {
        SteerInput = Vehicle.ReadInputParameter(VehicleInputParameter.Steer);

        var angle = SteerInput * MaxSteeringAngle/ (((Vehicle.ReadParameter(VehicleParameter.VehicleSpeed)+3)* VelocitySomething)+1);

        var angleDelta = angle - actualSteerAngle;
        angleDelta = Mathf.Clamp(angleDelta,-MaxAngularVelocity*Time.fixedDeltaTime, MaxAngularVelocity * Time.fixedDeltaTime);

        actualSteerAngle += angleDelta;

        if (Mode == SteeringMode.Simple)
        {
            LeftWheel.SteerAngle = actualSteerAngle;
            RightWheel.SteerAngle = actualSteerAngle;
            return;
        }

        float SteerAngle = actualSteerAngle;
        float radSteerAngle = Mathf.Deg2Rad * SteerAngle;

        float L = Mathf.Atan((WheelBase * Mathf.Sin(radSteerAngle)) / (WheelBase * Mathf.Cos(radSteerAngle) + Width * Mathf.Sin(radSteerAngle)));
        float R = Mathf.Atan((WheelBase * Mathf.Sin(radSteerAngle)) / (WheelBase * Mathf.Cos(radSteerAngle) - Width * Mathf.Sin(radSteerAngle)));

        if (Mode == SteeringMode.Ackermann)
        {
            LeftWheel.SteerAngle = Mathf.Rad2Deg * L;
            RightWheel.SteerAngle = Mathf.Rad2Deg * R;
        }
        else
        {
            LeftWheel.SteerAngle = Mathf.Rad2Deg * R;
            RightWheel.SteerAngle = Mathf.Rad2Deg * L;
        }
    }
}

public enum SteeringMode
{
    Simple,
    Ackermann,
    InverseAckermann
}