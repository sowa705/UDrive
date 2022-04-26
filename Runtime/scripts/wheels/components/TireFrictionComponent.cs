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

            var force = tire.CalculateLocalForce((float)longitudinalSlipRatio,(float)lateralSlipAngle,tickState.SuspensionForce);
            var globalForce = force.x * forwardDirection + force.y * lateralDirection;
            Collider.parentRB.AddForceAtPosition(globalForce / Collider.Vehicle.Substeps * Collider.LForceMultip, pos);

            Collider.debugData.FrictionForce = force;
            Collider.debugData.SlipAngle = (float)lateralSlipAngle;
            Collider.debugData.SlipRatio = (float)longitudinalSlipRatio + 1;
            reactionTorque = Collider.Parameters.Radius * force.x;
        }
        else
        {
            Collider.debugData.FrictionForce = Vector2.zero;
            Collider.debugData.SlipAngle = 0;
            Collider.debugData.SlipRatio = 0;
        }
        if (float.IsNaN(reactionTorque))
        {
            reactionTorque = 0;
        }

        tickState.ReactionTorque = reactionTorque;
    }
}