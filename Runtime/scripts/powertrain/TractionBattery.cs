using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class TractionBattery : VehicleComponent,IDebuggableComponent
    {
        /// <summary>
        /// Fully charged capacity in kWh, typical car battery is 50-100 kWh
        /// </summary>
        public float MaxEnergy = 60;
        float currentEnergy;
        /// <summary>
        /// Remaining energy in kWh
        /// </summary>
        public float CurrentEnergy
        {
            get => currentEnergy; set
            {
                if (value < 0)
                    currentEnergy = 0;
                else if (value > MaxEnergy)
                    currentEnergy = MaxEnergy;
                else
                    currentEnergy = value;
            }
        }

        public bool InitializeFull = true;

        public float ChargeValue { get => CurrentEnergy / MaxEnergy; }

        private void FixedUpdate()
        {
            vehicle.WriteParameter(VehicleParameter.TractionBatteryCharge,ChargeValue);
        }

        public override void VehicleStart()
        {
            if (InitializeFull)
            {
                CurrentEnergy = MaxEnergy;
            }
        }

        public void DrawDebugText()
        {
            GUILayout.Label($"Battery charge:{(ChargeValue*100).ToString("000.00")}% ({CurrentEnergy.ToString("000.00")} kWh)");
        }
    }
}