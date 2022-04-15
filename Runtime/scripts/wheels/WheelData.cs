using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WheelParameters
{
    public float Radius = 0.3f;
    public float Width = 0.2f;
    public float Mass = 20;
    public SuspensionSettings SuspensionSettings = new SuspensionSettings();
    public PacejkaTireData FrictionData = new PacejkaTireData();
}

[Serializable]
public class SuspensionSettings
{
    public float Spring = 3000;
    public float Damper = 1000;
    public float Travel = 0.3f;
}
[Serializable]
public class TireData
{
    public float LongitudalFrictionMultiplier;
    public AnimationCurve longitudalSlipCurve;
    public float LateralFrictionMultiplier;
    public AnimationCurve lateralSlipCurve;
}

[Serializable]

public class WheelDebugData
{
    public float SlipAngle;
    public float SlipRatio;

    public Vector2 FrictionForce;
}