using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakingController : VehicleComponent
{
    [Range(0f, 1f)]
    public float BrakeBias=0.5f;
    public float BrakeTorque = 5000;
    public float HandbrakeTorque = 1500;

    List<UWheelCollider> frontWheels = new List<UWheelCollider>();
    List<UWheelCollider> rearWheels = new List<UWheelCollider>();
    public override void VehicleStart()
    {
        var wheels = vehicle.GetWheels();

        foreach (var item in wheels)
        {
            if (item.transform.localPosition.z>0)
            {
                frontWheels.Add(item);
            }
            else
            {
                rearWheels.Add(item);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float torque = Vehicle.ReadInputParameter(VehicleParamId.BrakeInput)* BrakeTorque;
        foreach (var item in frontWheels)
        {
            item.BrakeTorque = (torque / frontWheels.Count) * (1 - BrakeBias);
        }
        float handbraketorque = Vehicle.ReadInputParameter(VehicleParamId.HandbrakeInput) * HandbrakeTorque;
        foreach (var item in rearWheels)
        {
            item.BrakeTorque = ((torque+ handbraketorque) / rearWheels.Count) * BrakeBias;
        }
    }
}
