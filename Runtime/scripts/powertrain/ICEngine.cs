using UnityEngine;
namespace UDrive
{
    public class ICEngine : VehicleComponent, ITorqueGenerator, IDebuggableComponent
    {
        [RequireInterface(typeof(ITorqueNode))]
        public Object Output;
        ITorqueNode outputGenerator { get => Output as ITorqueNode; }
        public float Torque = 100;
        public float MaxRPM = 5000;
        public float IdleRPM = 800;
        public float CurrentRPM;
        float cutoffTimer = 0;

        float currentTq;
        float currentPwr;
        float currentInput;

        PIDController idleRPMController;

        public void RunSubstep()
        {
            cutoffTimer -= vehicle.SubstepDeltaT;
            float input = Vehicle.ReadInputParameter(VehicleInputParameter.Accelerator);

            input += idleRPMController.ComputeStep(CurrentRPM, vehicle.SubstepDeltaT);

            input = Mathf.Clamp01(input);

            if (cutoffTimer > 0)
            {
                input = 0;
            }
            if (CurrentRPM > MaxRPM)
            {
                cutoffTimer = 0.1f;
            }

            float torque = Torque * input;
            torque -= (CurrentRPM / 100f) + (Torque / 100);
            currentInput = input;
            currentTq = torque;
            currentPwr = torque * CurrentRPM / 9.54f / 1000;

            Vehicle.WriteParameter(VehicleParameter.EngineRPM, CurrentRPM);

            CurrentRPM = outputGenerator.GetRPMFromTorque(torque);
        }

        public override void VehicleStart()
        {
            Vehicle.WriteParameter(VehicleParameter.EngineMaxRPM, MaxRPM);
            idleRPMController = new PIDController(3, 0.6f, 0);
            idleRPMController.InputDivider = 1000;
            idleRPMController.SetPoint = IdleRPM;
        }

        public void DrawDebugText()
        {
            GUILayout.Label($"Throttle: {currentInput.ToString("0.00")}  {CurrentRPM.ToString("0000")} RPM\ttorque: {currentTq.ToString("000")} Nm\tpower: {currentPwr.ToString("000")} kW");
        }
    }
}