using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameDebugger : VehicleComponent
{
    public override void VehicleStart()
    {
    }

    void OnGUI()
    {
        var wheels = vehicle.GetWheels();
        GUI.Box(new Rect(10, 10, 400, 45+ (wheels.Length+1)*25), $"UDrive - {vehicle.name}");
        GUI.Label(new Rect(20, 30, 400, 20),"Wheel name\tSuspension force\tSlip ratio\tSlip angle");
        float totalsusforce = 0;
        for (int i = 0; i < wheels.Length; i++)
        {
            totalsusforce += wheels[i].LastTickState.SuspensionForce;
            GUI.Label(new Rect(20, 50 + i * 25, 400, 20), $"{wheels[i].name}\t\t{(int)wheels[i].LastTickState.SuspensionForce} N\t\t{(wheels[i].debugData.SlipRatio).ToString("00.0")}\t{wheels[i].debugData.SlipAngle.ToString("00.0")} deg");
        }
        GUI.Label(new Rect(20, 50 + wheels.Length * 25, 400, 20), $"Total suspension force: {(int)totalsusforce} N ({-Mathf.RoundToInt(totalsusforce/Physics.gravity.y)} kg)");
    }
}
