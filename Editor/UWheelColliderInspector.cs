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

        GUILayout.Label($"Forward slip ratio: {collider.instance.ForwardSlipRatio}");
        GUILayout.Label($"Lateral slip angle: {collider.instance.SlipAngle}");
        GUILayout.Label($"Wheel RPM: {collider.instance.RPM}");
        GUILayout.Label($"Rotation angle: {collider.instance.RotationAngle}");
        GUILayout.Label($"Longitudinal force: {collider.instance.LongitudinalFrictionForce}");
        GUILayout.Label($"Lateral force: {collider.instance.LateralFrictionForce}");
        GUILayout.Label($"Suspension force: {collider.instance.SuspensionForce}");
    }
}