using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class Differential : PowertrainNode, IDebuggableComponent
    {
        public float FinalDriveRatio = 4;
        public DifferentialType Type;
        [Range(0f, 1f)]
        public float Bias = 0.5f;

        float ATorque;
        float BTorque;
        float TotalTq;

        public void DrawDebugText()
        {
            GUILayout.Label($"Diff total: {TotalTq.ToString("00000")} Nm, A {ATorque.ToString("0000")} Nm, B {BTorque.ToString("0000")} Nm");
        }

        public override float GetRPMFromTorque(float torque)
        {
            switch (Type)
            {
                case DifferentialType.Open:
                    return ProcessOpenDiff(torque);
            }
            throw new System.Exception("Invalid differential type");
        }

        float ProcessOpenDiff(float torque)
        {
            TotalTq = torque * FinalDriveRatio;
            float tq = TotalTq / (Outputs.Count / 2);

            ATorque = tq * (1 - Bias);
            BTorque = tq * (Bias);
            float rpm = 0;
            for (int i = 0; i < Outputs.Count / 2; i++)
            {
                rpm += GetOutput(i).GetRPMFromTorque(ATorque);
            }
            for (int i = Outputs.Count / 2; i < Outputs.Count; i++)
            {
                rpm += GetOutput(i).GetRPMFromTorque(BTorque);
            }
            return rpm / Outputs.Count * FinalDriveRatio;
        }
    }

    public enum DifferentialType
    {
        Open
    }
}