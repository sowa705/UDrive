namespace UDrive
{
    public class FixedGearbox : PowertrainNode
    {
        public float Ratio;

        public override float GetRPMFromTorque(float torque)
        {
            return GetOutput().GetRPMFromTorque(torque * Ratio) * Ratio;
        }
    }
}