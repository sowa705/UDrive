using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialEmissionLightController : VehicleComponent
    {
        public List<LightSlot> Slots;

        private MeshRenderer renderer;
        public string emissionParameter = "_EmissionColor";
        public override void VehicleStart()
        {
            renderer = GetComponent<MeshRenderer>();
        }

        private void FixedUpdate()
        {
            bool blink = (int)(Time.time * 2) % 2 == 0;
            foreach (var slot in Slots)
            {
                Color color=Color.black;
                
                switch (slot.Type)
                {
                    case LightType.Daytime:
                        if (vehicle.ReadInputParameter(VehicleInputParameter.HeadlightMode)>0)
                        {
                            color = Color.white;
                        }
                        break;
                    case LightType.Brake:
                        if (vehicle.ReadInputParameter(VehicleInputParameter.HeadlightMode)>0)
                        {
                            color = Color.red*0.5f;
                        }
                        goto case LightType.AuxBrake;
                    case LightType.AuxBrake:
                        if (vehicle.ReadInputParameter(VehicleInputParameter.Brake)>0.2f)
                        {
                            color = Color.red;
                        }
                        break;
                    case LightType.Headlight:
                        if (vehicle.ReadInputParameter(VehicleInputParameter.HeadlightMode)>1)
                        {
                            color = Color.white;
                        }
                        break;
                    case LightType.TurnLeft:
                        if (vehicle.ReadInputParameter(VehicleInputParameter.Blinkers)<0&&blink)
                        {
                            color = new Color(1, 0.5f, 0);
                        }
                        if (vehicle.ReadInputParameter(VehicleInputParameter.HazardLightsEnabled)>0&&blink)
                        {
                            color = new Color(1, 0.5f, 0);
                        }
                        break;
                    case LightType.TurnRight:
                        if (vehicle.ReadInputParameter(VehicleInputParameter.Blinkers)>0&&blink)
                        {
                            color = new Color(1, 0.5f, 0);
                        }
                        if (vehicle.ReadInputParameter(VehicleInputParameter.HazardLightsEnabled)>0&&blink)
                        {
                            color = new Color(1, 0.5f, 0);
                        }
                        break;
                    case LightType.Reverse:
                        if (vehicle.ReadParameter(VehicleParameter.CurrentGear)<0)
                        {
                            color = Color.white;
                        }
                        break;
                }
                
                renderer.materials[slot.MaterialSlot].SetColor(emissionParameter,color);
            }
        }
    }
}