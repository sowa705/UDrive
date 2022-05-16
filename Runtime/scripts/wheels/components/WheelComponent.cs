using System.Collections;
using System.Collections.Generic;
namespace UDrive
{
    /// <summary>
    /// Wheel functionality is divided between multiple components: suspension, friction and final torque calculations. WheelComponent is a base class for these features
    /// </summary>
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