using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameDebugger : VehicleComponent
{
    GUIStyle style;
    public override void VehicleStart()
    {
        style = new GUIStyle();
        style.normal.background = MakeColorTexture(64, 64, new Color(0,0,0,.5f));
    }

    void OnGUI()
    {
        var wheels = vehicle.GetWheels();
        GUILayout.BeginVertical(style);
        GUILayout.Label($"UDrive - {vehicle.name}");
        float totalsusforce = 0;
        GUILayout.Label("Wheel name\tSuspension force\tSlip ratio\tSlip angle     ");
        for (int i = 0; i < wheels.Length; i++)
        {
            totalsusforce += wheels[i].LastTickState.SuspensionForce;
            GUILayout.Label($"{wheels[i].name}\t\t{(int)wheels[i].LastTickState.SuspensionForce} N\t\t{(wheels[i].debugData.SlipRatio).ToString("00.00")}\t{(wheels[i].debugData.SlipAngle*Mathf.Rad2Deg).ToString("+0.00;-0.00")} °");
        }
        GUILayout.Label($"Total suspension force: {(int)totalsusforce} N ({-Mathf.RoundToInt(totalsusforce / Physics.gravity.y)} kg)");
        GUILayout.Label($"Velocity: {(vehicle.ReadParameter(VehicleParamId.VehicleSpeed)*3.6f).ToString("000.0")} km/h");
        GUILayout.Label($"Acceleration: {(vehicle.ReadParameter(VehicleParamId.VehicleLongitudinalAcceleration)).ToString("00.0")} m/s²\t{(vehicle.ReadParameter(VehicleParamId.VehicleLateralAcceleration)).ToString("00.0")} m/s²");
        GUILayout.Label($"Engine speed: {(vehicle.ReadParameter(VehicleParamId.EngineRPM)).ToString("0000")} RPM");
        GUILayout.EndVertical();
    }

    Texture2D MakeColorTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
