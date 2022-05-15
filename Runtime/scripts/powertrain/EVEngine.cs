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

        PIDController idleRPMController;

        public void RunSubstep()
        {
            float input = Vehicle.ReadInputParameter(VehicleInputParameter.Accelerator);

            input = Mathf.Clamp01(input);

            if (CurrentRPM >= MaxRPM)
            {
                input = 0;
            }

            float torque = MaxTorque * input;
            //torque -= (CurrentRPM / 100f);



            float tqrpm = Mathf.Clamp(CurrentRPM, 100, MaxRPM);

            currentMaxTq = MaxPower * 1000 * 9.54f / tqrpm;

            if (torque > currentMaxTq)
            {
                torque = currentMaxTq;
            }

            currentInput = input;
            currentTq = torque;

            currentPwr = torque * CurrentRPM / 9.54f / 1000;

            Vehicle.WriteParameter(VehicleParameter.EngineRPM, CurrentRPM);

            CurrentRPM = outputGenerator.GetRPMFromTorque(torque);
        }

        public override void VehicleStart()
        {
            Vehicle.WriteParameter(VehicleParameter.EngineMaxRPM, MaxRPM);
        }

        public void DrawDebugText()
        {
            GUILayout.Label($"Throttle: {currentInput.ToString("0.00")}  {CurrentRPM.ToString("0000")} RPM\ttorque: {currentTq.ToString("000")} Nm\tpower: {currentPwr.ToString("000")} kW ({currentMaxTq.ToString("000")} Nm max)");
        }
    }
}