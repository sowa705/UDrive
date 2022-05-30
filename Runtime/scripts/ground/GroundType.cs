using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    [Serializable]
    public class GroundType
    {
        public string TagName;
        [Range(0.1f,3f)]
        public float FrictionMultiplier = 1;
    }
}