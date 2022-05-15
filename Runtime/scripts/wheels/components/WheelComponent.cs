using System.Collections;
using System.Collections.Generic;
namespace UDrive
{
    abstract class WheelComponent
    {
        protected UWheelCollider Collider;
        public WheelComponent(UWheelCollider collider)
        {
            Collider = collider;
        }

        public abstract void RunSubstep(WheelTickState tickState, float deltaT);
    }
}