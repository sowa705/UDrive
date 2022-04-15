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

        double forwardVelocity = localVelocity.z;
        double lateralVelocity = localVelocity.x;

        double wheelVelocity = Collider.wheelState.AngularVelocity * Collider.Parameters.Radius;
        if (double.IsNaN(wheelVelocity))
        {
            wheelVelocity = 0;
        }

        Vector3 pos = Collider.transform.position;

        double longitudinalSlipRatio = (wheelVelocity - forwardVelocity) / (Math.Abs(forwardVelocity) + 0.1);
        double lateralSlipAngle = -Math.Atan(lateralVelocity / (Math.Abs(forwardVelocity) + 0.1));

        float reactionTorque = 0;


        if (tickState.IsGrounded)
        {
            SimplifiedPacejkaTireData tire = Collider.Parameters.FrictionData.Tire;
            float longitudinalForce = Collider.Parameters.FrictionData.LongitudinalMultiplier * tire.CalculateForce((float)longitudinalSlipRatio, tickState.SuspensionForce);
            float lateralForce = Collider.Parameters.FrictionData.LateralMultiplier * tire.CalculateForce((float)lateralSlipAngle / 10f, tickState.SuspensionForce);

            var force = forwardDirection * longitudinalForce + lateralDirection * lateralForce;

            Collider.parentRB.AddForceAtPosition(force / Collider.Vehicle.Substeps * Collider.LForceMultip, pos);

            Collider.debugData.FrictionForce = new Vector2(longitudinalForce, lateralForce);
            Collider.debugData.SlipAngle = (float)lateralSlipAngle;
            Collider.debugData.SlipRatio = (float)longitudinalSlipRatio;
            reactionTorque = Collider.Parameters.Radius * longitudinalForce;
        }

        if (float.IsNaN(reactionTorque))
        {
            reactionTorque = 0;
        }

        tickState.ReactionTorque = reactionTorque;
    }
}