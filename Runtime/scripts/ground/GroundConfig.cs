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

        private static GroundConfig currentConf;
        /// <summary>
        /// Returns the loaded global GroundConfig instance or a new instance
        /// </summary>
        public static GroundConfig GlobalConfig {
            get
            {
                if (currentConf==null)
                {
                    currentConf = Resources.Load<GroundConfig>("GlobalGroundConfig");
                    if (currentConf==null)
                    {
                        currentConf = ScriptableObject.CreateInstance<GroundConfig>();
                        
                        currentConf.GroundTypes.Add(new GroundType() { TagName = "Default", FrictionMultiplier = 1 });
                        currentConf.GroundTypes.Add(new GroundType() { TagName = "Ice", FrictionMultiplier = 0.5f });
                    }
                    return currentConf;
                }
                else
                {
                    return currentConf;
                }
            } }
    }
}