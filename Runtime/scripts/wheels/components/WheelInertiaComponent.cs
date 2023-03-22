using UnityEngine;
namespace UDrive
{
    /// <summary>
    /// Standard inertia component, calculates final wheel torque and applies it
    /// </summary>
    class WheelInertiaComponent : WheelComponent
    {
        public WheelInertiaComponent(UWheelCollider collider) : base(collider)
        {
        }

        public override void RunSubstep(WheelTickState tickState, float deltaT)
        {
            float moi = Collider.Parameters.Mass * Collider.Parameters.Radius;

            float finaltorque = Collider.EngineTorque - tickState.ReactionTorque;

            Collider.wheelState.AngularVelocity += (finaltorque / moi) * deltaT;
            float brakeTq = Collider.BrakeTorque + Collider.wheelState.AngularVelocity * 0.1f; //brake torque + friction

            brakeTq = -Mathf.Sign(Collider.wheelState.AngularVelocity) * (brakeTq);

            bool sign = Collider.wheelState.AngularVelocity > 0;

            Collider.wheelState.AngularVelocity += (brakeTq / moi) * deltaT;

            if (Collider.wheelState.AngularVelocity > 0 != sign) //we crossed the 0 line
            {
                Collider.wheelState.AngularVelocity = 0;
            }

            if (Collider.wheelState.AngularVelocity < 0.05f && brakeTq > 0)
            {
                Collider.wheelState.AngularVelocity = 0;
            }
            
            Collider.wheelState.RotationAngle += Collider.wheelState.AngularVelocity * 9.5493f * deltaT * 6;
            Collider.wheelState.RotationAngle = Collider.wheelState.RotationAngle % 360f;
        }
        
        public override void OnDetach()
        {
            //nothing to do
        }
    }
}