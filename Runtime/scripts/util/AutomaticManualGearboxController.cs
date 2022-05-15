using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class AutomaticManualGearboxController : VehicleComponent
    {
        float LastShiftTimer;
        ManualGearbox gearbox;
        public override void VehicleStart()
        {
            gearbox = GetComponentInChildren<ManualGearbox>();
        }

        void FixedUpdate()
        {
            LastShiftTimer -= Time.fixedDeltaTime;

            if (LastShiftTimer > 0)
                return;
            if (gearbox.Clutch < 0.5f)
            {
                LastShiftTimer = 0.2f;
                return;
            }
            float rpm = Vehicle.ReadParameter(VehicleParameter.EngineRPM);
            float shiftRPM = Vehicle.ReadParameter(VehicleParameter.EngineMaxRPM);
            if (rpm > shiftRPM * 0.9f)
            {
                gearbox.ShiftGear(gearbox.Gear + 1);
                LastShiftTimer = 1f;
            }
            if (rpm < shiftRPM * 0.25f && gearbox.Gear > 1)
            {
                gearbox.ShiftGear(gearbox.Gear - 1);
                LastShiftTimer = 1f;
            }

        }
        private void LateUpdate()
        {
            if (gearbox.IsShifting)
            {
                Vehicle.WriteInputParameter(VehicleInputParameter.Accelerator, 0);
                //Vehicle.WriteInputParameter(VehicleParamId.ClutchInput, 1);
            }
            else
            {
                //Vehicle.WriteInputParameter(VehicleParamId.ClutchInput, 0);
            }
        }
    }
}