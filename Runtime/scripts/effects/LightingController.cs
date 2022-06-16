using UnityEngine;

namespace UDrive
{
    public class LightingController:VehicleComponent
    {
        public Light FL;
        public Light FR;

        public Light RL;
        public Light RR;


        public float LowBeamIntensity=2f;
        public float HighBeamIntensity=8f;

        void FixedUpdate()
        {
            FL.color=Color.white;
            FR.color=Color.white;
            RL.color=Color.red;
            RR.color=Color.red;
            bool blink = (int)(Time.time * 2) % 2 == 0;
            int headlightmode = (int)vehicle.ReadInputParameter(VehicleInputParameter.HeadlightMode);
            FL.range = 25;
            FR.range = 25;
            switch (headlightmode)
            {
                case 0:
                    FL.intensity = 0;
                    FR.intensity = 0;
                    RL.intensity = 0;
                    RR.intensity = 0;
                    break;
                case 1:
                    FL.intensity = 0.2f;
                    FR.intensity = 0.2f;
                    RL.intensity = 0.5f;
                    RR.intensity = 0.5f;
                    break;
                case 2:
                    FL.intensity = LowBeamIntensity*0.5f;
                    FR.intensity = LowBeamIntensity;
                    RL.intensity = 1f;
                    RR.intensity = 1f;
                    break;
                case 3:
                    FL.range = 65;
                    FR.range = 65;
                    FL.intensity = HighBeamIntensity;
                    FR.intensity = HighBeamIntensity;
                    RL.intensity = 1;
                    RR.intensity = 1;
                    break;
            }

            if (vehicle.ReadInputParameter(VehicleInputParameter.Brake)>0.2f)
            {
                RL.intensity = 2;
                RR.intensity = 2;
            }

            var hazards = vehicle.ReadInputParameter(VehicleInputParameter.HazardLightsEnabled) > 0;

            if (hazards||vehicle.ReadInputParameter(VehicleInputParameter.Blinkers)>0)
            {
                if (headlightmode<2)
                {
                    FR.color = blink?new Color(1, 0.5f, 0):FR.color;
                    FR.intensity = blink?1:FR.intensity;
                }
                RR.color = blink?new Color(1, 0.5f, 0):RR.color;
                RR.intensity = blink?1:RR.intensity;
            }
            if (hazards||vehicle.ReadInputParameter(VehicleInputParameter.Blinkers)<0)
            {
                if (headlightmode<2)
                {
                    FL.color = blink?new Color(1, 0.5f, 0):FL.color;
                    FL.intensity = blink?1:FL.intensity;
                }
                RL.color = blink?new Color(1, 0.5f, 0):RL.color;
                RL.intensity = blink?1:RL.intensity;
            }
        }
    }
}