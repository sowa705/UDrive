using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputController : VehicleComponent, IDebuggableComponent
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
        Vehicle.WriteInputParameter(VehicleInputParameter.Steer,Input.GetAxisRaw("Horizontal"));
        Vehicle.WriteInputParameter(VehicleInputParameter.Accelerator, Input.GetKey(KeyCode.W)?1:0);
        Vehicle.WriteInputParameter(VehicleInputParameter.Brake, Input.GetKey(KeyCode.S) ? 1 : 0);
        Vehicle.WriteInputParameter(VehicleInputParameter.Handbrake, Input.GetKey(KeyCode.Space) ? 1 : 0);
        Vehicle.WriteInputParameter(VehicleInputParameter.Clutch, Input.GetKey(KeyCode.E) ? 1 : 0);

        if (Input.GetKeyDown(KeyCode.P))
        {
            CruiseControlSpeed += 10;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CruiseControlSpeed -= 10;
            if (CruiseControlSpeed<0)
            {
                CruiseControlSpeed = 0;
            }
        }

        if (CruiseControlSpeed>0)
        {
            cruiseControl.SetPoint = CruiseControlSpeed/3.6f;
            float input = cruiseControl.ComputeStep(vehicle.ReadParameter(VehicleParameter.VehicleSpeed),vehicle.CurrentDeltaT);
            Vehicle.WriteInputParameter(VehicleInputParameter.Accelerator, input);
        }
    }

    public void DrawDebugText()
    {
        GUILayout.Label($"Cruise control setpoint: {CruiseControlSpeed}");
    }
}
public enum InputType
{
    InputManager,
    None
}