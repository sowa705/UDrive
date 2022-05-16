using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UDrive
{
    public class SingleWheelSteeringController: VehicleComponent
    {
        [Range(0,45)]
        public float MaxAngle;

        public UWheelCollider WheelCollider;

        private void FixedUpdate()
        {
            WheelCollider.SteerAngle = MaxAngle * Vehicle.ReadInputParameter(VehicleInputParameter.Steer);
        }
    }
}
