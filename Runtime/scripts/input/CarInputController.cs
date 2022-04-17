using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputController : VehicleComponent
{
    public InputType Type;

    public override void VehicleStart()
    {
    }
    // Update is called once per frame
    void Update()
    {
        ProcessIMInput();
    }
    void ProcessIMInput()
    {
        Vehicle.WriteParameter(VehicleParamId.SteeringInput,Input.GetAxis("Horizontal"));
        Vehicle.WriteParameter(VehicleParamId.AcceleratorInput, Input.GetKey(KeyCode.W)?1:0);
        Vehicle.WriteParameter(VehicleParamId.BrakeInput, Input.GetKey(KeyCode.S) ? 1 : 0);
    }
}
public enum InputType
{
    InputManager,
    None
}