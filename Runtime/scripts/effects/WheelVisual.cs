using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelVisual : MonoBehaviour
{
    UWheelCollider wcollider;
    public Transform Pivot;
    public Vector3 PivotAxis;
    public Transform Wheel;
    public Vector3 WheelAxis;
    public Vector3 WheelSteerAxis;

    Quaternion startPivotRotation;
    Quaternion startWheelRotation;
    Vector3 wheelOffset;

    // Start is called before the first frame update
    void Start()
    {
        wcollider = GetComponent<UWheelCollider>();
        if (Pivot != null)
        {
            startPivotRotation = Pivot.transform.localRotation;
        }
        startWheelRotation = Wheel.transform.localRotation;
        wheelOffset = Wheel.transform.localPosition;
    }

    void Update()
    {
        if (Pivot != null)
        {
            Pivot.transform.localRotation = startPivotRotation * Quaternion.Euler(PivotAxis * wcollider.SteerAngle);
            Wheel.transform.localRotation = startWheelRotation * Quaternion.Euler(WheelAxis * wcollider.wheelState.RotationAngle);

        }
        else
        {
            Wheel.transform.localRotation = startWheelRotation * Quaternion.Euler(WheelSteerAxis * wcollider.SteerAngle)*Quaternion.Euler(WheelAxis * wcollider.wheelState.RotationAngle) ;
            Wheel.transform.localPosition= wcollider.GetLocalWheelPosition() + wheelOffset;
        }
    }
}
