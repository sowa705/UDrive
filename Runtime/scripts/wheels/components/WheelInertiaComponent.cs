using UnityEngine;

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
        float brakeTq = Collider.BrakeTorque;

        brakeTq = -Mathf.Sign(Collider.wheelState.AngularVelocity) * (brakeTq + 5);
        bool sign = Collider.wheelState.AngularVelocity > 0;

        Collider.wheelState.AngularVelocity += (brakeTq / moi) * deltaT;

        if (Collider.wheelState.AngularVelocity > 0 != sign) //we crossed the 0 line
        {
            Collider.wheelState.AngularVelocity = 0;
        }

        Collider.wheelState.AngularVelocity -= Collider.wheelState.AngularVelocity / 10f * deltaT; //damping

        Collider.wheelState.RotationAngle += Collider.wheelState.AngularVelocity * 9.5493f * deltaT * 6;
        Collider.wheelState.RotationAngle = Collider.wheelState.RotationAngle % 360f;
    }
}