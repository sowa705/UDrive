﻿using UnityEngine;
namespace UDrive
{
    /// <summary>
    /// Standard UDrive suspension, raycast based and somewhat stable in the general case
    /// </summary>
    class RaycastSuspensionComponent : WheelComponent
    {
        public RaycastSuspensionComponent(UWheelCollider collider) : base(collider)
        {
        }

        public override void RunSubstep(WheelTickState tickState, float deltaT)
        {
            Vector3 dir = Collider.transform.up;
            Vector3 pos = Collider.GetWheelPosition();
            float suspensionCollisionForce = 0;
            tickState.IsGrounded = false;


            Vector3 springPos = pos + (dir * Collider.Parameters.SuspensionSettings.Travel * 1);

            Vector3 rayPos = Collider.GetWheelPosition();
            float len = Collider.Parameters.Radius;
            tickState.IsGrounded = false;

            Color c = Color.yellow;
            RaycastHit hit;

            string tagname = "";

            if (Physics.Raycast(new Ray(rayPos, -dir), out hit, Collider.Parameters.Radius))
            {
                float depth = len - hit.distance;
                suspensionCollisionForce = ((depth / Collider.Parameters.Radius) / (deltaT * deltaT));
                tickState.IsGrounded = true;
                c = Color.magenta;
                tagname = hit.collider.tag;
            }

            Debug.DrawRay(rayPos + (Collider.transform.forward * 0.1f), -Collider.transform.forward * 0.2f, c, 0.2f);

            Debug.DrawRay(rayPos, -dir * len, Color.yellow, 0.2f);

            float springForce = 1 * Collider.Parameters.SuspensionSettings.Spring * Collider.wheelState.SuspensionPosition;
            float damperForce = 1 * Collider.wheelState.SuspensionVelocity * Collider.Parameters.SuspensionSettings.Damper;

            float force = springForce + damperForce;

            float maxForce = Collider.Vehicle.Rigidbody.mass * 10;

            force = Mathf.Clamp(force, -maxForce, maxForce);

            Vector3 rbForce = dir * force;

            Collider.parentRB.AddForceAtPosition(rbForce / Collider.Vehicle.Substeps, springPos, ForceMode.Force);

            Collider.wheelState.SuspensionVelocity -= ((force - suspensionCollisionForce) * deltaT) / Collider.Parameters.Mass;
            Collider.wheelState.SuspensionVelocity = Mathf.Clamp(Collider.wheelState.SuspensionVelocity, -5, 5);
            Collider.wheelState.SuspensionPosition += Collider.wheelState.SuspensionVelocity * deltaT;
            Collider.wheelState.SuspensionPosition = Mathf.Clamp(Collider.wheelState.SuspensionPosition, -Collider.Parameters.SuspensionSettings.Travel * 1, Collider.Parameters.SuspensionSettings.Travel * 1);
            tickState.TagName = tagname;
            tickState.SuspensionForce = (force);
        }

        public override void OnDetach()
        {
            // nothing to do here
        }
    }
}