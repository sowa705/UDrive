namespace UDrive
{
    public class Flywheel : VehicleComponent, ITorqueNode
    {
        public float MOI;
        public float RPM;

        public float SubstepTorque;

        public float GetRPMFromTorque(float torque)
        {
            SubstepTorque = torque;
            RPM += torque / MOI / 50f;
            RPM -= RPM / 10000f;
            return RPM;
        }

        public override void VehicleStart()
        {
        }
    }
}