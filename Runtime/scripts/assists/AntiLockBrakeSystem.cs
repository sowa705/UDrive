using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UDrive
{
    public class AntiLockBrakeSystem : VehicleComponent, IVehicleAssist,IDebuggableComponent
    {
        [Range(0.1f,0.9f)]
        public float TargetSlipRatio=0.25f;
        bool Activated;
        public void OnWheel(UWheelCollider collider)
        {
            if (collider.BrakeTorque>0)
            {
                if (collider.LastTickState.SlipRatio< TargetSlipRatio)
                {
                    collider.BrakeTorque = 0;
                    Activated = true;
                }
            }
        }

        public void DrawDebugText()
        {
            GUILayout.Label($"ABS activated: {Activated}");
        }

        public void OnUpdate()
        {
            Vehicle.WriteParameter(VehicleParameter.ABSActive, Activated ? 1 : 0);
            Activated = false;
        }
    }
}