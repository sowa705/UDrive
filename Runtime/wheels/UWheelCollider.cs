using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UWheelCollider : MonoBehaviour,ITorqueNode
{
    public WheelData WheelData;
    [NonSerialized]
    public WheelInstance instance;
    Rigidbody parentRB;
    WheelCollider SuspensionCollider;
    UVehicle parentVehicle;

    public float EngineTorque;
    public float BrakeTorque;

    public float LForceMultip;
    private bool ApplyNonVerticalSuspensionForces;
    private bool ApplySuspensionTorque;

    private void Start()
    {
        ResetWheel();
    }
    void ResetWheel()
    {
        parentRB = GetComponentInParent<Rigidbody>();
        parentVehicle=GetComponentInParent<UVehicle>();
        /*
        if (SuspensionCollider!=null)
        {
            Destroy(SuspensionCollider);
        }

        instance = new WheelInstance() { Data = WheelData };

        SuspensionCollider = gameObject.AddComponent<WheelCollider>();
        SuspensionCollider.radius = WheelData.Radius;
        SuspensionCollider.mass= WheelData.Mass;
        SuspensionCollider.suspensionDistance = WheelData.SuspensionSettings.Distance;

        // zero friction, we will apply these forces ourselves
        SuspensionCollider.forwardFriction= new WheelFrictionCurve { asymptoteSlip = 0, asymptoteValue = 0, extremumSlip = 0, extremumValue = 0, stiffness = 0 };
        SuspensionCollider.sidewaysFriction= new WheelFrictionCurve { asymptoteSlip = 0, asymptoteValue = 0, extremumSlip = 0, extremumValue = 0, stiffness = 0 };

        SuspensionCollider.suspensionSpring = new JointSpring { damper = WheelData.SuspensionSettings.Damper, spring = WheelData.SuspensionSettings.Spring, targetPosition = 0.5f };*/
    }
    Vector3 GetWheelPosition()
    {
        return transform.position + instance.SuspensionPosition * transform.up;
    }
    void SuspensionUpdate(float deltaT)
    {
        Vector3 dir = transform.TransformDirection(Vector3.up);
        Vector3 pos = GetWheelPosition();
        float suspensionCollisionForce = 0;
        instance.Grounded = false;


        Vector3 springPos = pos + (dir * instance.Data.SuspensionSettings.Distance * 1);

        Vector3 rayPos = GetWheelPosition();
        float len = instance.Data.Radius;

        Debug.DrawRay(rayPos, -dir * len, Color.yellow, 0.2f);

        foreach (var hit in Physics.RaycastAll(rayPos, -dir, len))
        {
            if (hit.collider.GetComponentInParent<UVehicle>() == parentVehicle)
            {
                continue;
            }
            Debug.DrawRay(rayPos, -dir* hit.distance, Color.cyan, 0.2f);

            float depth = len - hit.distance;
            instance.Grounded = true;

            if (depth < 0)
            {
                depth = 0;
                continue;
            }
            Debug.DrawRay(pos - dir * instance.Data.Radius, dir * depth, Color.red, 0.2f);

            suspensionCollisionForce = ((depth * instance.Data.Mass) / (deltaT * deltaT))  * 50;
        }



        float springForce = 1 * instance.Data.SuspensionSettings.Spring * instance.SuspensionPosition;
        float damperForce = 1 * instance.SuspensionVelocity * instance.Data.SuspensionSettings.Damper;

        float force = springForce + damperForce;

        force = Mathf.Clamp(force, -8000, 8000);

        Vector3 rbForce = dir * force;

        parentRB.AddForceAtPosition(rbForce / parentVehicle.Substeps, springPos, ForceMode.Force);

        instance.SuspensionVelocity -= ((force - suspensionCollisionForce) * deltaT) / instance.Data.Mass;
        instance.SuspensionVelocity = Mathf.Clamp(instance.SuspensionVelocity, -5, 5);
        instance.SuspensionPosition += instance.SuspensionVelocity * deltaT;
        instance.SuspensionPosition = Mathf.Clamp(instance.SuspensionPosition, -instance.Data.SuspensionSettings.Distance * 1, instance.Data.SuspensionSettings.Distance * 1);
        instance.SuspensionForce = (force);
    }

    void OnDrawGizmos()
    {
        if (instance==null)
        {
            OnValidate();
        }
        Vector3 dir = transform.up;
        Vector3 worldPos = transform.position;
        Vector3 normal = transform.right;

        Vector3 rayPos = worldPos;

        Vector3 rotDir = transform.TransformDirection(Quaternion.Euler(instance.RotationAngle, 0, 0) * Vector3.forward);

        float rayDist = instance.Data.Radius;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(rayPos, rayPos - dir * rayDist);

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(rayPos, rayPos - rotDir * rayDist);

#if UNITY_EDITOR
        Handles.DrawWireDisc(transform.position,transform.right,instance.Data.Radius);
#endif
    }
    private void OnValidate()
    {
        instance = new WheelInstance() { Data = WheelData };
    }
    void SuspensionUpdate()
    {
        instance.Grounded = SuspensionCollider.isGrounded;
        WheelHit hit;
        if (SuspensionCollider.GetGroundHit(out hit))
        {
            instance.SuspensionForce = hit.force*3;
        }
        else
        {
            instance.SuspensionForce = 0;
        }
    }

    void FrictionUpdate(float deltaT)
    {
        Vector3 force = new Vector3();

        //Calculate the wheel's linear velocity
        Vector3 worldVelocity = (instance.WorldPosition - instance.LastWorldPosition) / Time.fixedDeltaTime;

        Vector3 localVelocity = Quaternion.Euler(0, -instance.SteerAngle, 0) * transform.InverseTransformVector(worldVelocity);

        Vector3 forwardDirection = Quaternion.Euler(0, instance.SteerAngle, 0) * transform.forward;
        Vector3 lateralDirection = Quaternion.Euler(0, instance.SteerAngle, 0) * transform.right;

        double forwardVelocity = localVelocity.z;
        double lateralVelocity = localVelocity.x;

        double wheelVelocity = instance.AngularVelocity * instance.Data.Radius;
        if (double.IsNaN(wheelVelocity))
        {
            wheelVelocity = 0;
        }

        Vector3 pos = transform.position;

        double longitudinalSlipRatio = (wheelVelocity- forwardVelocity) / Math.Abs(forwardVelocity) + 0.01;
        double lateralSlipAngle = -Math.Atan(lateralVelocity/ Math.Abs(forwardVelocity) + 0.01);

        float reactionTorque = 0;


        if (instance.Grounded)
        {
            SimplifiedPacejkaTireData tire = instance.Data.TireData.Tire;
            float longitudinalForce = instance.Data.TireData.LongitudinalMultiplier * tire.CalculateForce((float)longitudinalSlipRatio, instance.SuspensionForce);
            float lateralForce = instance.Data.TireData.LateralMultiplier * tire.CalculateForce((float)lateralSlipAngle / 10f, instance.SuspensionForce);

            force = forwardDirection * longitudinalForce + lateralDirection * lateralForce;

            parentRB.AddForceAtPosition(force / parentVehicle.Substeps* LForceMultip, pos);

            instance.LongitudinalFrictionForce = longitudinalForce / parentVehicle.Substeps;
            instance.LateralFrictionForce = lateralForce / parentVehicle.Substeps;

            reactionTorque = instance.Data.Radius * longitudinalForce;

            instance.ForwardSlipRatio = (float)longitudinalSlipRatio;
            instance.SlipAngle = (float)lateralSlipAngle;
        }
        else
        {
            instance.ForwardSlipRatio = 0;
            instance.LateralSlip = 0;
            instance.SlipAngle = 0;
        }


        instance.ReactionTorque = reactionTorque;
        instance.Torque -= reactionTorque;
    }
    void WheelInertiaUpdate(float deltaT)
    {
        float moi = instance.Data.Mass * instance.Data.Radius;

        instance.AngularVelocity += (instance.Torque / moi) * deltaT;
        float brakeTq = instance.BrakeTorque;

        brakeTq = -Mathf.Sign(instance.AngularVelocity) * (brakeTq + 5);
        bool sign = instance.AngularVelocity > 0;

        instance.AngularVelocity += (brakeTq / moi) * deltaT;

        if (instance.AngularVelocity > 0 != sign) //we crossed the 0 line
        {
            instance.AngularVelocity = 0;
        }

        instance.AngularVelocity -= instance.AngularVelocity / 10f * deltaT; //damping

        instance.RPM = instance.AngularVelocity * 9.5493f;

        instance.RotationAngle = instance.RotationAngle % 360f;
    }

    public void RunSubstep(float deltaT)
    {
        instance.LongitudinalFrictionForce = 0;
        instance.LateralFrictionForce = 0;
        instance.Torque = EngineTorque;
        instance.BrakeTorque = BrakeTorque;

        SuspensionUpdate(deltaT);
        FrictionUpdate(deltaT);
        WheelInertiaUpdate(deltaT);
    }
    void FixedUpdate()
    {
        instance.LastWorldPosition = instance.WorldPosition;
        instance.WorldPosition = transform.position;
    }
    public float GetRPMFromTorque(float torque)
    {
        EngineTorque = torque;
        return instance.RPM;
    }
}
