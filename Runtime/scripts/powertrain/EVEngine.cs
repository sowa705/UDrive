using UnityEngine;

namespace UDrive
{
    public class EVEngine : VehicleComponent, ITorqueGenerator, IDebuggableComponent
    {
        [RequireInterface(typeof(ITorqueNode))]
        public Object Output;
        ITorqueNode outputGenerator { get => Output as ITorqueNode; }
        public float MaxTorque = 100;
        public float MaxPower = 200;
        public float MaxRPM = 5000;
        public float CurrentRPM;

        float currentTq;
        float currentPwr;
        float currentInput;
        float currentMaxTq;

        public bool EnableRegen;

        TractionBattery battery;

        public void RunSubstep()
        {
            float input = Vehicle.ReadInputParameter(VehicleInputParameter.Accelerator);

            input = Mathf.Clamp01(input);

            if (CurrentRPM >= MaxRPM)
            {
                input = 0;
            }

            if (battery != null&&battery.ChargeValue<=0)
            {
                input = 0;
            }

            if (EnableRegen&&CurrentRPM> (MaxRPM/50f))
            {
                input -= Vehicle.ReadInputParameter(VehicleInputParameter.Brake);
            }

            float torque = MaxTorque * input;
            //torque -= (CurrentRPM / 100f);



            float tqrpm = Mathf.Clamp(CurrentRPM, 100, MaxRPM);

            currentMaxTq = MaxPower * 1000 * 9.54f / tqrpm;

            if (Mathf.Abs(torque) > currentMaxTq)
            {
                torque = Mathf.Sign(torque)*currentMaxTq;
            }

            currentInput = input;
            currentTq = torque;

            currentPwr = torque * CurrentRPM / 9.54f / 1000;

            

            if (battery != null)
            {
                float currentenergy = currentPwr * vehicle.SubstepDeltaT / 3600f; //kWh for this frame

                battery.CurrentEnergy -= currentenergy;
            }
            
            Vehicle.WriteParameter(VehicleParameter.EngineRPM, CurrentRPM);

            CurrentRPM = outputGenerator.GetRPMFromTorque(torque);
        }

        public override void VehicleStart()
        {
            Vehicle.WriteParameter(VehicleParameter.EngineMaxRPM, MaxRPM);

            if (battery==null)
            {
                battery = vehicle.GetComponentInChildren<TractionBattery>();
            }
        }

        public void DrawDebugText()
        {
            GUILayout.Label($"Throttle: {currentInput.ToString("0.00")}  {CurrentRPM.ToString("0000")} RPM\ttorque: {currentTq.ToString("000")} Nm\tpower: {currentPwr.ToString("000")} kW ({currentMaxTq.ToString("000")} Nm max)");
        }
    }
}