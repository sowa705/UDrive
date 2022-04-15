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
    
    void LateUpdate()
    {
        if (Target==null)
        {
            return;
        }
        var targetTransform = Target.transform;

        Vector3 position = new Vector3(0, Elevation, -Distance);
        float rotationAngle = Mathf.Atan(Elevation / Distance)*Mathf.Rad2Deg;

        var targetposition = targetTransform.TransformPoint(position);
        var targetrotation = Quaternion.LookRotation(targetTransform.forward,targetTransform.up)* Quaternion.Euler(rotationAngle* AngleMultiplier, 0, 0);

        transform.position = Vector3.Slerp(transform.position,targetposition,Time.unscaledDeltaTime* 10f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, Time.unscaledDeltaTime*6f);
    }
}
