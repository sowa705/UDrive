﻿using UnityEngine;

public class Engine : MonoBehaviour, ITorqueGenerator
{
    [RequireInterface(typeof(ITorqueNode))]
    public Object Output;

    ITorqueNode outputGenerator { get => Output as ITorqueNode; }
    public float Torque = 100;
    public float MaxRPM = 5000;
    public float CurrentRPM;
    public void RunSubstep()
    {
        float torque=Torque;
        if (CurrentRPM > MaxRPM)
        {
            torque = 0;
        }

        CurrentRPM=outputGenerator.GetRPMFromTorque(torque);
    }
}