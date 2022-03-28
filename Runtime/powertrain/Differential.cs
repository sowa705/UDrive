using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Differential : PowertrainNode
{
    public float FinalDriveRatio=4;
    public DifferentialType Type;
    public override float GetRPMFromTorque(float torque)
    {
        switch (Type)
        {
            case DifferentialType.Open:
                return ProcessOpenDiff(torque);
        }
        throw new System.Exception("Invalid differential type");
    }

    float ProcessOpenDiff(float torque)
    {
        float tq = torque / Outputs.Count * FinalDriveRatio;
        float rpm = 0;
        for (int i = 0; i < Outputs.Count; i++)
        {
            rpm+=GetOutput(i).GetRPMFromTorque(tq);
        }
        return rpm / Outputs.Count * FinalDriveRatio;
    }
}

public enum DifferentialType
{
    Open
}