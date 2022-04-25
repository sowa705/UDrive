using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UWheelCollider : VehicleComponent, ITorqueNode, IStatefulComponent
{
    public WheelParameters Parameters;
    //[NonSerialized]
    public WheelState wheelState;
    public WheelTickState LastTickState { get; private set; }
    [NonSerialized]
    public Rigidbody parentRB;

    //[NonSerialized]
    public WheelDebugData debugData;

    public float EngineTorque;
    public float BrakeTorque;

    public float LForceMultip;

    public float SteerAngle;

    Vector3 lastworldpos;
    Vector3 worldpos;

    public Vector3 Velocity;
    List<WheelComponent> wheelComponents=new List<WheelComponent>();

    void ResetWheel()
    {
        wheelState = new WheelState();

        wheelComponents.Clear();

        wheelComponents.Add(new RaycastSuspensionComponent(this));
        wheelComponents.Add(new TireFrictionComponent(this));
        wheelComponents.Add(new WheelInertiaComponent(this));

        parentRB = GetComponentInParent<Rigidbody>();
    }
    public Vector3 GetWheelPosition()
    {
        return transform.position + wheelState.SuspensionPosition * transform.up;
    }

    void OnDrawGizmos()
    {
        if (wheelState==null)
        {
            OnValidate();
        }
        Vector3 dir = transform.up;
        Vector3 worldPos = transform.position;
        Vector3 normal = Quaternion.Euler(0, SteerAngle, 0) * transform.right;

        Vector3 rayPos = GetWheelPosition();

        Vector3 rotDir = Quaternion.Euler(wheelState.RotationAngle, SteerAngle, 0) * transform.forward;

        float rayDist = Parameters.Radius;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(rayPos, rayPos - dir * rayDist);

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(rayPos, rayPos - rotDir * rayDist);

#if UNITY_EDITOR
        Handles.DrawWireDisc(rayPos, normal, Parameters.Radius);
#endif
    }
    private void OnValidate()
    {
        wheelState = new WheelState();
    }

    public void RunSubstep(float deltaT)
    {
        LastTickState = new WheelTickState();

        foreach (var item in wheelComponents)
        {
            item.RunSubstep(LastTickState, deltaT);
        }
    }
    void FixedUpdate()
    {
        lastworldpos = worldpos;
        worldpos = transform.position;
        Velocity =  (worldpos-lastworldpos)/Time.fixedDeltaTime;
    }
    public float GetRPMFromTorque(float torque)
    {
        EngineTorque = torque;
        return wheelState.AngularVelocity * 9.5493f;
    }

    public override void VehicleStart()
    {
        ResetWheel();
    }

    public void SerializeState(BinaryWriter writer)
    {
        writer.Write(wheelState.SuspensionPosition);
        writer.Write(wheelState.SuspensionVelocity);
        writer.Write(wheelState.AngularVelocity);
    }

    public void Deserialize(BinaryReader reader)
    {
        wheelState.SuspensionPosition = reader.ReadSingle();
        wheelState.SuspensionVelocity = reader.ReadSingle();
        wheelState.AngularVelocity = reader.ReadSingle();
    }
}
