using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    [CreateAssetMenu(menuName = "UDrive/Ground config", fileName = "GlobalGroundConfig")]
    public class GroundConfig : ScriptableObject
    {
        public List<GroundType> GroundTypes = new List<GroundType>();
        [Range(0.5f, 2f)]
        public float GlobalMultiplier=1;
        public float GetFrictionMultiplier(string tagName)
        {
            foreach (var item in GroundTypes)
            {
                if (item.TagName==tagName)
                {
                    return item.FrictionMultiplier * GlobalMultiplier;
                }
            }

            return GlobalMultiplier;
        }
        /// <summary>
        /// Returns the loaded global GroundConfig instance or a new instance
        /// </summary>
        public static GroundConfig GlobalConfig { get { var gc = Resources.Load<GroundConfig>("GlobalGroundConfig"); return gc ?? new GroundConfig(); } }
    }
}