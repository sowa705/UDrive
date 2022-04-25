using UnityEngine;

public class Engine : VehicleComponent, ITorqueGenerator
{
    [RequireInterface(typeof(ITorqueNode))]
    public Object Output;
    ITorqueNode outputGenerator { get => Output as ITorqueNode; }
    public float Torque = 100;
    public float MaxRPM = 5000;
    public float CurrentRPM;
    public void RunSubstep()
    {
        float torque = Torque * Vehicle.ReadInputParameter(VehicleParamId.AcceleratorInput);
        torque -= CurrentRPM / 500f;
        if (CurrentRPM > MaxRPM)
        {
            torque = 0;
        }

        CurrentRPM=outputGenerator.GetRPMFromTorque(torque);
        Vehicle.WriteParameter(VehicleParamId.EngineRPM,CurrentRPM);
    }

    public override void VehicleStart()
    {
        Vehicle.WriteParameter(VehicleParamId.EngineMaxRPM, MaxRPM);
    }
}
