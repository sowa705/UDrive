using UnityEngine;
namespace UDrive
{
    /// <summary>
    /// Test alternative suspension based on PhysX WheelCollider
    /// </summary>
    class NativeSuspensionComponent : WheelComponent
    {
        WheelCollider SuspensionCollider;
        public NativeSuspensionComponent(UWheelCollider collider) : base(collider)
        {
            SuspensionCollider = Collider.gameObject.AddComponent<WheelCollider>();
            SuspensionCollider.radius = Collider.Parameters.Radius;
            SuspensionCollider.mass = Collider.Parameters.Mass;
            SuspensionCollider.suspensionDistance = Collider.Parameters.SuspensionSettings.Travel;

            // zero friction, we will apply these forces ourselves
            SuspensionCollider.forwardFriction = new WheelFrictionCurve { asymptoteSlip = 0, asymptoteValue = 0, extremumSlip = 0, extremumValue = 0, stiffness = 0 };
            SuspensionCollider.sidewaysFriction = new WheelFrictionCurve { asymptoteSlip = 0, asymptoteValue = 0, extremumSlip = 0, extremumValue = 0, stiffness = 0 };

            SuspensionCollider.suspensionSpring = new JointSpring { damper = Collider.Parameters.SuspensionSettings.Damper, spring = Collider.Parameters.SuspensionSettings.Spring, targetPosition = 0.5f };
        }

        public override void RunSubstep(WheelTickState tickState, float deltaT)
        {
            WheelHit hit;
            if (SuspensionCollider.GetGroundHit(out hit))
            {
                tickState.SuspensionForce = hit.force;
                tickState.IsGrounded = true;
            }
            else
            {
                tickState.SuspensionForce = 0;
                tickState.IsGrounded = false;
            }
        }

        public override void OnDetach()
        {
            Object.Destroy(SuspensionCollider);
        }
    }
}