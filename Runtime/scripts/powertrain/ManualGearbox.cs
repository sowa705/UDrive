using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualGearbox : PowertrainNode, ITorqueNode
{
    public List<float> GearRatios = new List<float>();
    public int Gear;

    public float FlywheelRPM = 0;
    public float ShaftRPM = 0;
    [Range(0, 1)]
    public float Clutch = 0;
    public float ActualClutch = 1;
    public float SmoothActualClutch;
    public float MaxClutchTorque = 200;
    public float AppliedClutchTQ;
    public bool AutoClutch;
    [Range(0.01f, 0.2f)]
    public float MomentOfInertia = 0.02f;
    float ShiftTimer;
    [Range(0.1f, 1f)]
    public float ShiftTime = 0.5f;
    public bool IsShifting { get => ShiftTimer > 0f; }

    float GetRatio()
    {
        if (Gear == -1)
        {
            return GearRatios[0];
        }
        else if (Gear == 0)
        {
            return 0;
        }
        else
        {
            return GearRatios[Gear];
        }
    }

    public void ShiftGear(int gear)
    {
        if (gear >= GearRatios.Count)
        {
            return;
        }
        Gear = gear;
        Vehicle.WriteParameter(VehicleParamId.CurrentGear, gear);
        ShiftTimer = ShiftTime;
    }
    public override float GetRPMFromTorque(float torque)
    {
        ShiftTimer -= vehicle.CurrentDeltaT;
        if (AutoClutch)
        {
            float targetrpm = 1000;
            float aclutchdiff = targetrpm - FlywheelRPM;
            ActualClutch = 1 - Mathf.Clamp01(aclutchdiff * 3 / targetrpm);
            if (Clutch < ActualClutch)
            {
                ActualClutch = Clutch;
            }
        }
        else
        {
            ActualClutch = Clutch;
        }
        if (ShiftTimer > 0)
        {
            ActualClutch = 0;
        }

        SmoothActualClutch = Mathf.Lerp(SmoothActualClutch, ActualClutch, vehicle.CurrentDeltaT * 10f);

        float diff = ShaftRPM - FlywheelRPM;
        if (Gear == 0)
            diff = 0;
        float ClutchTq = Mathf.Clamp01(Mathf.Abs(diff) / 300f) * MaxClutchTorque * Mathf.Sign(diff) * SmoothActualClutch;

        float tq = torque + ClutchTq;
        FlywheelRPM += (tq / MomentOfInertia) * vehicle.CurrentDeltaT;

        AppliedClutchTQ = ClutchTq;

        ShaftRPM = GetOutput().GetRPMFromTorque(-ClutchTq*GetRatio()) * GetRatio();

        return FlywheelRPM;
    }
    public override void PowertrainStart()
    {
        Vehicle.WriteParameter(VehicleParamId.CurrentGear, Gear);
    }
}
