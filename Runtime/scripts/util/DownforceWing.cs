using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class DownforceWing : VehicleComponent
    {
        public float Downforce;
        // Update is called once per frame
        void FixedUpdate()
        {
            float speed = vehicle.ReadParameter(VehicleParameter.VehicleSpeed);
            vehicle.Rigidbody.AddForceAtPosition(Vector3.down * (0.00119f * Downforce * speed * speed), transform.position);
        }
    }
}