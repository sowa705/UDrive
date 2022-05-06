using UnityEngine;

public class ICEngine : VehicleComponent, ITorqueGenerator
{
    [RequireInterface(typeof(ITorqueNode))]
    public Object Output;
    ITorqueNode outputGenerator { get => Output as ITorqueNode; }
    public float Torque = 100;
    public float MaxRPM = 5000;
    public float CurrentRPM;
    float cutoffTimer = 0;
    public void RunSubstep()
    {
        cutoffTimer-=vehicle.CurrentDeltaT;
        float input =  Vehicle.ReadInputParameter(VehicleParamId.AcceleratorInput);
        if (cutoffTimer>0)
        {
            input = 0;
            Vehicle.WriteInputParameter(VehicleParamId.AcceleratorInput, 0);
        }
        if (CurrentRPM > MaxRPM)
        {
            cutoffTimer = 0.1f;
        }
        float torque = Torque * input;
        torque -= CurrentRPM / 100f;

        Vehicle.WriteParameter(VehicleParamId.EngineRPM, CurrentRPM);

        CurrentRPM = outputGenerator.GetRPMFromTorque(torque);
    }

    public override void VehicleStart()
    {
        Vehicle.WriteParameter(VehicleParamId.EngineMaxRPM, MaxRPM);
    }
}
