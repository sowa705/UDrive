using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UDrive
{
    public class VehicleResetter : VehicleComponent
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position += Vector3.up * 10f;
                transform.rotation = Quaternion.identity;

                vehicle.ResetVehicle();
            }
        }
    }
}