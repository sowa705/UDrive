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
    //public SuspensionState SuspensionState;
    //public SuspensionDerivative SuspensionDerivative;
    public float SuspensionPosition;
    public float SuspensionVelocity;
    public float SuspensionForce;

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

[Serializable]
public class SuspensionState
{
    public float Position;
    public float Velocity;
}

[Serializable]
public class SuspensionDerivative
{
    public float Velocity;
    public float Acceleration;
}

public static class SuspensionRK4Helpers
{
    public static void Integrate(SuspensionSettings settings,SuspensionState state, float t,float deltaT)
    {
        SuspensionDerivative a, b, c, d;

        a = Evaluate(settings, state, t, 0.0f,new SuspensionDerivative());
        b = Evaluate(settings, state, t, deltaT * 0.5f, a);
        c = Evaluate(settings, state, t, deltaT * 0.5f, b);
        d = Evaluate(settings, state, t, deltaT, c);

        float dxdt = 1.0f / 6.0f *
            (a.Velocity + 2.0f * (b.Velocity + c.Velocity) + d.Velocity);

        float dvdt = 1.0f / 6.0f *
            (a.Acceleration + 2.0f * (b.Acceleration + c.Acceleration) + d.Acceleration);

        state.Position = state.Position + dxdt * deltaT;
        state.Velocity = state.Velocity + dvdt * deltaT;
    }

    static SuspensionDerivative Evaluate(SuspensionSettings settings,SuspensionState initial, float t, float deltaT, SuspensionDerivative derivative)
    {
        SuspensionState state=new SuspensionState();
        state.Position = initial.Position + derivative.Velocity * deltaT;
        state.Velocity = initial.Velocity + derivative.Acceleration * deltaT;

        SuspensionDerivative output=new SuspensionDerivative();
        output.Velocity = state.Velocity;
        output.Acceleration = GetSpringAcceleration(state, settings, t + deltaT);
        return output;
    }

    static float GetSpringAcceleration(SuspensionState state,SuspensionSettings settings,float deltaT)
    {
        return -settings.Spring * state.Position - settings.Damper * state.Velocity;
    }
}