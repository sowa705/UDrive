using UnityEngine;

namespace UDrive
{
    [CreateAssetMenu(menuName = "UDrive/Wheel Parameters", fileName = "Wheel parameters")]
    public class WheelParametersObject : ScriptableObject
    {
        public WheelParameters Parameters;
    }
}