using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UDrive
{
    internal class TractionControl : VehicleComponent, IDebuggableComponent, IVehicleAssist
    {
        bool Activated;
        [Range(1f,2f)]
        public float TargetMaxSlip = 1.25f;
        public void DrawDebugText()
        {
            GUILayout.Label($"TCS activated: {Activated}");
            Vehicle.WriteParameter(VehicleParameter.TCSActive,Activated?1:0);
        }

        public void OnUpdate()
        {
            Activated = false;

            foreach (var item in vehicle.GetWheels())
            {
                if (item.debugData.SlipRatio>TargetMaxSlip)
                {
                    Vehicle.WriteInputParameter(VehicleInputParameter.Accelerator,0);
                    Activated = true;
                }
            }
        }

        public void OnWheel(UWheelCollider collider)
        {
        }
    }
}
