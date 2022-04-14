public interface ITorqueGenerator
{
    void RunSubstep();
}

public interface ITorqueNode
{
    float GetRPMFromTorque(float torque);
}