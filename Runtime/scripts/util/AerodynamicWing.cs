using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicWing : VehicleComponent
{
    public float Downforce;
    // Update is called once per frame
    void FixedUpdate()
    {
        vehicle.Rigidbody.AddForceAtPosition(Vector3.down*Downforce*vehicle.ReadParameter(VehicleParameter.VehicleSpeed),transform.position);
    }
}
