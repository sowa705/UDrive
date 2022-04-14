using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
[CanEditMultipleObjects]
[CustomEditor(typeof(UWheelCollider))]
public class UWheelColliderInspector:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UWheelCollider collider = target as UWheelCollider;
        /*
        GUILayout.Label($"Forward slip ratio: {collider.wheelState.ForwardSlipRatio}");
        GUILayout.Label($"Lateral slip angle: {collider.wheelState.SlipAngle}");
        GUILayout.Label($"Wheel RPM: {collider.wheelState.RPM}");
        GUILayout.Label($"Rotation angle: {collider.wheelState.RotationAngle}");
        GUILayout.Label($"Longitudinal force: {collider.wheelState.LongitudinalFrictionForce}");
        GUILayout.Label($"Lateral force: {collider.wheelState.LateralFrictionForce}");
        GUILayout.Label($"Suspension force: {collider.wheelState.SuspensionForce}");*/
    }
}