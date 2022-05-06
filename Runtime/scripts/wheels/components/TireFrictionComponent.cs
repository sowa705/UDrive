using System;
using UnityEngine;

class TireFrictionComponent : WheelComponent
{
    public TireFrictionComponent(UWheelCollider collider) : base(collider)
    {
    }

    public override void RunSubstep(WheelTickState tickState, float deltaT)
    {
        Quaternion wheelRotation = Quaternion.Euler(0, Collider.SteerAngle, 0);
        Quaternion invWheelRotation = Quaternion.Euler(0, -Collider.SteerAngle, 0);

        Vector3 localVelocity = invWheelRotation * Collider.transform.InverseTransformVector(Collider.Velocity);

        Vector3 forwardDirection = wheelRotation * Collider.transform.forward;
        Vector3 lateralDirection = wheelRotation * Collider.transform.right;

        float forwardVelocity = localVelocity.z;
        float lateralVelocity = localVelocity.x;

        float wheelVelocity = Collider.wheelState.AngularVelocity * Collider.Parameters.Radius;
        if (float.IsNaN(wheelVelocity))
        {
            wheelVelocity = 0;
        }

        Vector3 pos = Collider.transform.position;

        float longitudinalSlipRatio = (wheelVelocity - forwardVelocity) / (Mathf.Abs(forwardVelocity) + 0.02f);
        float lateralSlipAngle = -Mathf.Atan(lateralVelocity / (Mathf.Abs(forwardVelocity) + 0.02f));

        float blendRatio = (Mathf.Abs(forwardVelocity) - 1) / 4f;
        blendRatio = Mathf.Clamp01(blendRatio);
        float lowSpeedLateralSlip = (-lateralVelocity) / 3f;

        float delta = (wheelVelocity - forwardVelocity) - Mathf.Abs(forwardVelocity);
        delta /= 0.91f;
        float differentialSlipRatio = delta * deltaT;

        float tau = 0.01f;
        float slipRatio = differentialSlipRatio + tau * delta;

        float finalSlipRatio = Mathf.Lerp(slipRatio, longitudinalSlipRatio, blendRatio);
        float finalSlipAngle = Mathf.Lerp(lowSpeedLateralSlip, lateralSlipAngle, blendRatio);

        float reactionTorque = 0;


        if (tickState.IsGrounded)
        {
            SimplifiedPacejkaTireData tire = Collider.Parameters.FrictionData.Tire;

            var force = tire.CalculateLocalForce(finalSlipRatio, finalSlipAngle, tickState.SuspensionForce);
            var globalForce = force.x * forwardDirection + force.y * lateralDirection;
            Collider.parentRB.AddForceAtPosition(globalForce / Collider.Vehicle.Substeps * Collider.LForceMultip, pos);

            Collider.debugData.FrictionForce = force;
            Collider.debugData.SlipAngle = finalSlipAngle;
            Collider.debugData.SlipRatio = finalSlipRatio + 1;
            Collider.debugData.BlendRatio = blendRatio;
            reactionTorque = Collider.Parameters.Radius * force.x;
        }
        else
        {
            Collider.debugData.FrictionForce = Vector2.zero;
            Collider.debugData.SlipAngle = 0;
            Collider.debugData.SlipRatio = 0;
            Collider.debugData.BlendRatio = 0;
        }
        Collider.debugData.Velocity = new Vector2(forwardVelocity, lateralVelocity);

        if (float.IsNaN(reactionTorque))
        {
            reactionTorque = 0;
        }

        tickState.ReactionTorque = reactionTorque;
    }
}