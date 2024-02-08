using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace UDrive
{
    [Serializable]
    public class WheelState
    {
        public float SuspensionPosition;
        /// <summary>
        /// Suspension velocity in m/s
        /// </summary>
        public float SuspensionVelocity;
        /// <summary>
        /// Wheel angular velocity in radians/second
        /// </summary>
        public float AngularVelocity;
        /// <summary>
        /// Wheel rotation angle in degrees, wraps around
        /// </summary>
        public float RotationAngle;
    }

    [Serializable]
    public class WheelTickState
    {
        /// <summary>
        /// Vertical suspension force
        /// </summary>
        public float SuspensionForce; //N
        /// <summary>
        /// Is the wheel touching the ground
        /// </summary>
        public bool IsGrounded;
        /// <summary>
        /// Torque transferred to the ground
        /// </summary>
        public float ReactionTorque; //Nm

        /// <summary>
        /// Lateral slip angle in radians
        /// </summary>
        public float SlipAngle;
        /// <summary>
        /// Longitudinal slip ratio, 0 means a completely locked up wheel, 1 is perfectly rolling wheel
        /// </summary>
        public float SlipRatio;
        /// <summary>
        /// Ratio between low-speed and high-speed slip calculations, 0 is low-speed, 1 is high-speed
        /// </summary>
        public float BlendRatio;
        /// <summary>
        /// Wheel local velocity, x = longitudinal, y = lateral
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Final friction forces, x = longitudinal, y = lateral
        /// </summary>
        public Vector2 FrictionForce;
        /// <summary>
        /// Tag name of the surface the wheel touches
        /// </summary>
        public string TagName;
        public int ColliderCount;
    }
}