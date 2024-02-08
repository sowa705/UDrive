using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UDrive
{
    [Serializable]
    public class WheelParameters
    {
        public float Radius = 0.3f;
        public float Width = 0.2f;
        public float Mass = 20;
        public SuspensionSettings SuspensionSettings = new SuspensionSettings();
        public SimpleTireFrictionCurve FrictionCurve = new SimpleTireFrictionCurve();
        /// <summary>
        /// Mesh used for the wheel collider in 1x1x1 meter scale. will be scaled to the wheel radius by UWheelCollider
        /// </summary>
        public Mesh ColliderMesh;
        /// <summary>
        /// Determines if the collider mesh should be rotated to match the wheel rotation, this is useful for weird meshes like square wheels :)
        /// </summary>
        public bool RotateColliderMesh = false;
    }

    [Serializable]
    public class SuspensionSettings
    {
        public float Spring = 40000;
        public float Damper = 2000;
        public float Travel = 0.3f;
    }
    [Serializable]
    public class TireData
    {
        public float LongitudalFrictionMultiplier;
        public AnimationCurve longitudalSlipCurve;
        public float LateralFrictionMultiplier;
        public AnimationCurve lateralSlipCurve;
    }
}