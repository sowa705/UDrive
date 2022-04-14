using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WheelState
{
    public float SuspensionPosition;
    public float SuspensionVelocity; //m/s

    public float AngularVelocity; //radians/sec
    public float RotationAngle; //degrees
}

[Serializable]
public class WheelTickState
{
    public float SuspensionForce; //N
    public bool IsGrounded;
    public float ReactionTorque; //Nm
}