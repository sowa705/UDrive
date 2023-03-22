using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class IngameDebugger : VehicleComponent
    {
        GUIStyle style;

        public bool DisplayInputs;
        public bool DisplayValues;

        public bool DisplayWheels;
        public override void VehicleStart()
        {
            style = new GUIStyle();
            style.normal.background = MakeColorTexture(64, 64, new Color(0, 0, 0, .5f));
        }

        void OnGUI()
        {
            var wheels = vehicle.GetWheels();
            GUILayout.BeginVertical(style);
            GUILayout.Label($"UDrive - {vehicle.name}");
            float totalsusforce = 0;

            if (DisplayWheels)
                GUILayout.Label("Wheel name\tSuspension force\tSlip ratio\tSlip angle\tBlend ratio\tRPM");
            for (int i = 0; i < wheels.Length; i++)
            {
                totalsusforce += wheels[i].LastTickState.SuspensionForce;
                if (DisplayWheels)
                    GUILayout.Label($"{wheels[i].name}\t\t{(int)wheels[i].LastTickState.SuspensionForce} N\t\t{(wheels[i].LastTickState.SlipRatio).ToString("00.00")}\t{(wheels[i].LastTickState.SlipAngle * Mathf.Rad2Deg).ToString("+0.00;-0.00")} °\t{(wheels[i].LastTickState.BlendRatio * 100).ToString("000")} %\t{(wheels[i].wheelState.AngularVelocity*9.549f).ToString("0000")} RPM");
            }

            float weight = totalsusforce / Physics.gravity.y;
            GUILayout.Label($"Total sus force: {totalsusforce.ToString("00000")} N ({Mathf.RoundToInt(-weight).ToString("0000")} kgf), Vehicle mass: {(vehicle.Rigidbody.mass).ToString("0000")} kg, diff: {(-Mathf.RoundToInt(weight + vehicle.Rigidbody.mass)).ToString("0000")} kgf");
            GUILayout.Label($"Velocity: {(vehicle.ReadParameter(VehicleParameter.VehicleSpeed) * 3.6f).ToString("000.0")} km/h, accel: forward: {(vehicle.ReadParameter(VehicleParameter.VehicleLongitudinalAcceleration)).ToString("00.0")} m/s²\tlateral: {(vehicle.ReadParameter(VehicleParameter.VehicleLateralAcceleration)).ToString("00.0")} m/s²");
            GUILayout.Label($"Road grade: {(vehicle.ReadParameter(VehicleParameter.RoadGrade)).ToString("00.0")} %");

            foreach (var item in vehicle.DebuggableComponents)
            {
                item.DrawDebugText();
            }

            if (DisplayInputs)
            {
                foreach (var item in vehicle.InputParameters)
                {
                    GUILayout.Label($"{item.Key}: {item.Value.ToString("0.00")}");
                }
            }

            if (DisplayValues)
            {
                foreach (var item in vehicle.VehicleValues)
                {
                    GUILayout.Label($"{item.Key}: {item.Value.ToString("0.000")}");
                }
            }
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
}