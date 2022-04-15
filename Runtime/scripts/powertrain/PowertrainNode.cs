using System.Collections.Generic;
using UnityEngine;

public class PowertrainNode : VehicleComponent, ITorqueNode
{
    //[RequireInterface(typeof(ITorqueNode))]
    public List<Object> Outputs;
    public ITorqueNode GetOutput(int index=0)
    {
        if (Outputs[index] is ITorqueNode)
        {
            return Outputs[index] as ITorqueNode;
        }
        else
        {
            return (Outputs[index] as GameObject).GetComponent<ITorqueNode>();
        }
    }
    public virtual float GetRPMFromTorque(float torque)
    {
        throw new System.NotImplementedException();
    }

    public override void VehicleStart()
    {
    }
}