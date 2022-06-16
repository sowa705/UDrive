namespace UDrive
{
    public enum VehicleInputParameter
    {
        /// <summary>
        /// Vehicle steering
        /// -1 = left, 1=right
        /// </summary>
        Steer=1,
        Accelerator,
        Brake,
        Handbrake,
        Clutch,
        /// <summary>
        /// 0 - disabled
        /// 1 - daytime lights
        /// 2 - low beam
        /// 3 - high beam
        /// </summary>
        HeadlightMode,
        /// <summary>
        /// Turn signals
        /// -1 = left, 1=right
        /// </summary>
        Blinkers,
        HazardLightsEnabled
    }
}