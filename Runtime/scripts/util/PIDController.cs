using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UDrive
{
    [Serializable]
    public class PIDController
    {
        public float ProportionalGain = 1;
        public float IntegralGain = .5f;
        public float DerivativeGain = 0.2f;

        public float InputDivider = 1;

        public float SetPoint;

        float integral;
        float lastError;

        public float OutputMin = 0;
        public float OutputMax = 1;

        public PIDController(float proportionalGain, float integralGain, float derivativeGain)
        {
            ProportionalGain = proportionalGain;
            IntegralGain = integralGain;
            DerivativeGain = derivativeGain;
        }

        public float ComputeStep(float input, float deltaT)
        {
            float proportionalError = (SetPoint - input) / InputDivider;

            integral += proportionalError * deltaT;

            integral = Mathf.Clamp(integral, -1, 1);

            float derivative = (proportionalError - lastError) / deltaT;

            lastError = proportionalError;

            float cv = proportionalError * ProportionalGain + integral * IntegralGain + derivative * DerivativeGain;

            return Mathf.Clamp(cv, OutputMin, OutputMax);
        }
    }
}