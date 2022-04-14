using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameDebugger : MonoBehaviour
{
    UVehicle vehicle;
    // Start is called before the first frame update
    void Start()
    {
        vehicle = GetComponent<UVehicle>();
    }

    void OnGUI()
    {
        /*
        var wheels = vehicle.GetWheels();
        GUI.Box(new Rect(10, 10, 400, 45+ (wheels.Length+1)*25), $"UDrive - {vehicle.name}");
        GUI.Label(new Rect(20, 30, 400, 20),"Wheel name\tSuspension force\tSlip ratio\tSlip angle");
        float totalsusforce = 0;
        for (int i = 0; i < wheels.Length; i++)
        {
            totalsusforce += wheels[i].wheelState.SuspensionForce;
            GUI.Label(new Rect(20, 50+i*25, 400, 20), $"{wheels[i].name}\t\t{(int)wheels[i].wheelState.SuspensionForce} N\t\t{((int)wheels[i].wheelState.ForwardSlipRatio).ToString("00.0")}\t{wheels[i].wheelState.SlipAngle.ToString("00.0")} deg");
        }
        GUI.Label(new Rect(20, 50 + wheels.Length * 25, 400, 20), $"Total suspension force: {(int)totalsusforce} N ({(int)-(totalsusforce/Physics.gravity.y)} kg)");
    */
    }
}
