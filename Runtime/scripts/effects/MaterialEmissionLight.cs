using System;

namespace UDrive
{
    [Serializable]
    public class LightSlot
    {
        public LightType Type;
        public int MaterialSlot;
    }

    public enum LightType
    {
        Daytime,
        Brake,
        AuxBrake,
        Headlight,
        TurnLeft,
        TurnRight,
        Reverse
    }
}