using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WheelInstance
{
    public WheelData Data;
    public float SuspensionPosition;
    public float SuspensionVelocity; //m/s
    public float SuspensionForce; //newtons
    public bool Grounded;

    public float Torque;
    public float BrakeTorque;
    public float SteerAngle;

    public Vector3 WorldPosition;
    public Vector3 LastWorldPosition;

    public float AngularVelocity; //radians/sec
    public float RPM;
    public float RotationAngle; //degrees

    public float ForwardSlipRatio;
    public float SlipAngle;
    public float LateralSlip;

    public float LongitudinalFrictionForce;
    public float LateralFrictionForce;

    public float ReactionTorque;
}