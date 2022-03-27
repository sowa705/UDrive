using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WheelData
{
    public float Radius = 0.3f;
    public float Width = 0.2f;
    public float Mass = 20;
    public SuspensionSettings SuspensionSettings = new SuspensionSettings();
    public PacejkaTireData TireData = new PacejkaTireData();
}

[Serializable]
public class SuspensionSettings
{
    public float Spring = 3000;
    public float Damper = 1000;
    public float Distance = 0.3f;
}
[Serializable]
public class TireData
{
    public float LongitudalFrictionMultiplier;
    public AnimationCurve longitudalSlipCurve;
    public float LateralFrictionMultiplier;
    public AnimationCurve lateralSlipCurve;
}