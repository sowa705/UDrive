using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputController : VehicleComponent
{
    public InputType Type;

    public float CruiseControlSpeed;

    PIDController cruiseControl;
    public override void VehicleStart()
    {
        cruiseControl = new PIDController(2,0.5f,0);
    }
    // Update is called once per frame
    void Update()
    {
        ProcessIMInput();
    }
    void ProcessIMInput()
    {
        Vehicle.WriteInputParameter(VehicleParamId.SteeringInput,Input.GetAxisRaw("Horizontal"));
        Vehicle.WriteInputParameter(VehicleParamId.AcceleratorInput, Input.GetKey(KeyCode.W)?1:0);
        Vehicle.WriteInputParameter(VehicleParamId.BrakeInput, Input.GetKey(KeyCode.S) ? 1 : 0);
        Vehicle.WriteInputParameter(VehicleParamId.HandbrakeInput, Input.GetKey(KeyCode.Space) ? 1 : 0);
        Vehicle.WriteInputParameter(VehicleParamId.ClutchInput, Input.GetKey(KeyCode.E) ? 1 : 0);

        if (CruiseControlSpeed>0)
        {
            cruiseControl.SetPoint = CruiseControlSpeed/3.6f;
            float input = cruiseControl.ComputeStep(vehicle.ReadParameter(VehicleParamId.VehicleSpeed),vehicle.CurrentDeltaT);
            Vehicle.WriteInputParameter(VehicleParamId.AcceleratorInput, input);
        }
    }
}
public enum InputType
{
    InputManager,
    None
}