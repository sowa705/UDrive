using System;
using UnityEngine;
namespace UDrive
{
    /// <summary>
    /// Standard friction component
    /// </summary>
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

            float longitudinalSlipRatio = (wheelVelocity - forwardVelocity) / (Mathf.Abs(forwardVelocity) + 0.1f);
            float lateralSlipAngle = -Mathf.Atan(lateralVelocity / (Mathf.Abs(forwardVelocity) + 0.1f));

            float blendRatio = (Mathf.Abs(forwardVelocity) - 1) / 4f;
            blendRatio = Mathf.Clamp01(blendRatio);
            float lowSpeedLateralSlip = (-lateralVelocity) / 8f;

            //obviously wrong but works fine for very low speeds
            float lowSpeedSlipRatio = (wheelVelocity - forwardVelocity)/7;

            float finalSlipRatio = Mathf.Lerp(lowSpeedSlipRatio, longitudinalSlipRatio, blendRatio);
            float finalSlipAngle = Mathf.Lerp(lowSpeedLateralSlip, lateralSlipAngle, blendRatio);

            float reactionTorque = 0;


            if (tickState.IsGrounded)
            {
                SimpleTireFrictionCurve tire = Collider.Parameters.FrictionCurve;

                var force = tire.CalculateLocalForce(finalSlipRatio, finalSlipAngle, tickState.SuspensionForce)*GroundConfig.GlobalConfig.GetFrictionMultiplier(tickState.TagName);

                var globalForce = force.x * forwardDirection + force.y * lateralDirection;
                Collider.parentRB.AddForceAtPosition(globalForce / Collider.Vehicle.Substeps * Collider.LForceMultip, pos);

                tickState.FrictionForce = force;
                tickState.SlipAngle = finalSlipAngle;
                tickState.SlipRatio = finalSlipRatio + 1;
                tickState.BlendRatio = blendRatio;
                reactionTorque = Collider.Parameters.Radius * force.x;
            }
            else
            {
                tickState.FrictionForce = Vector2.zero;
                tickState.SlipAngle = 0;
                tickState.SlipRatio = 0;
                tickState.BlendRatio = 0;
            }
            Collider.LastTickState.Velocity = new Vector2(forwardVelocity, lateralVelocity);

            if (float.IsNaN(reactionTorque))
            {
                reactionTorque = 0;
            }

            tickState.ReactionTorque = reactionTorque;
        }
    }
}