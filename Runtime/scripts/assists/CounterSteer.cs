using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class CounterSteerController:VehicleComponent,IVehicleAssist,IDebuggableComponent
    {
        public void OnWheel(UWheelCollider collider)
        {
            
        }

        public void DrawDebugText()
        {
        }

        public void OnUpdate()
        {
        }
    }
}