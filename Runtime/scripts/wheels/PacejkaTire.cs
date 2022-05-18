using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UDrive
{
    [Serializable]
    public class SimpleTireData
    {
        [Range(0, 3)]
        public float LongitudinalMultiplier = 1;
        [Range(0, 3)]
        public float LateralMultiplier = 1;
        public SimpleTireFrictionCurve Tire;
    }

    //default values taken from https://www.edy.es/dev/docs/pacejka-94-parameters-explained-a-comprehensive-guide/
    [Serializable]
    public class SimpleTireFrictionCurve
    {
        [Range(4, 12)]
        public float Stiffness = 10;
        [Range(1, 2)]
        public float Shape = 1.9f;
        [Range(0.1f, 1.9f)]
        public float Peak = 1;
        [Range(-10, 1)]
        public float Curvature = 0.97f;

        public float GetMaxCof()
        {
            return Peak;
        }

        public float CalculateCoF(float slip)
        {
            float bslip = slip * Stiffness;

            return Peak * Mathf.Sin(Shape * Mathf.Atan(bslip - Curvature * (bslip - Mathf.Atan(bslip))));
        }

        public Vector2 CalculateLocalForce(float slipRatio, float slipAngle, float SuspensionForce)
        {
            float maxCOF = GetMaxCof();
            float ratio = Mathf.Sqrt(Mathf.Pow(slipRatio / maxCOF, 2) + Mathf.Pow(slipAngle / maxCOF, 2));
            float forwardCoF = CalculateCoF(slipRatio);
            float lateralCoF = CalculateCoF(slipAngle);

            return new Vector2(((slipRatio / maxCOF) /ratio)*CalculateCoF(ratio), ((slipAngle / maxCOF) / ratio) * CalculateCoF(ratio)) * SuspensionForce;
        }
    }
}