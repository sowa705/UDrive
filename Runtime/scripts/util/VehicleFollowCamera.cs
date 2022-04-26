using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class VehicleFollowCamera : MonoBehaviour
{
    public UVehicle Target;
    [Range(1f, 20f)]
    public float Distance=10;
    [Range(0f, 6f)]
    public float Elevation=3;
    [Range(0f, 1f)]
    public float AngleMultiplier=0.25f;
    [Range(5f, 50f)]
    public float FollowSpeed = 10f;
    [Range(5f, 25f)]
    public float RotationSpeed = 10f;

    public bool AvoidCollision;
    public bool LookBack;
    void LateUpdate()
    {
        if (Target==null)
        {
            return;
        }
        float actualRotSpeed = RotationSpeed;

        if (LookBack)
        {
            actualRotSpeed = 200f;
        }

        LookBack = Input.GetKey(KeyCode.Q);

        var targetTransform = Target.transform;

        Vector3 position = new Vector3(0, Elevation, -Distance);
        if (LookBack)
        {
            position = new Vector3(0, Elevation, Distance);
            actualRotSpeed = 200f;
        }



        float rotationAngle = Mathf.Atan(Elevation / Distance)*Mathf.Rad2Deg;

        var targetposition = targetTransform.TransformPoint(position);
        var targetrotation = Quaternion.LookRotation(targetTransform.forward, targetTransform.up) * Quaternion.Euler(rotationAngle * AngleMultiplier, 0, 0);

        if (LookBack)
            targetrotation = Quaternion.LookRotation(-targetTransform.forward, targetTransform.up) * Quaternion.Euler(rotationAngle * AngleMultiplier, 0, 0);


        if (AvoidCollision)
        {
            RaycastHit hit;

            Ray r = new Ray(Target.transform.position, targetposition - Target.transform.position);
            float length = (targetposition - Target.transform.position).magnitude;
            LayerMask mask = Physics.DefaultRaycastLayers & (~LayerMask.GetMask("IgnoreCameraRaycast"));

            Debug.DrawRay(r.origin, r.direction * length, Color.red);

            if (Physics.Raycast(r, out hit, length,mask))
            {
                Debug.DrawLine(r.origin,hit.point,Color.green);
                targetposition = hit.point;
            }
        }
        
        transform.position = Vector3.Slerp(transform.position,targetposition,Time.unscaledDeltaTime* FollowSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, Time.unscaledDeltaTime* actualRotSpeed);
    }
}
