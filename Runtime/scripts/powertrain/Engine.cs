using UnityEngine;

public class Engine : VehicleComponent, ITorqueGenerator
{
    [RequireInterface(typeof(ITorqueNode))]
    public Object Output;
    UVehicle vehicle;
    ITorqueNode outputGenerator { get => Output as ITorqueNode; }
    public float Torque = 100;
    public float MaxRPM = 5000;
    public float CurrentRPM;
    public void Start()
    {
        vehicle = GetComponentInParent<UVehicle>();
    }
    public void RunSubstep()
    {
        float torque = Torque * Vehicle.ReadParameter(VehicleParamId.AcceleratorInput);
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
