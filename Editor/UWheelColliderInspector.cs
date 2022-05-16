using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
namespace UDrive
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UWheelCollider))]
    public class UWheelColliderInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            VehicleComponent component = target as VehicleComponent;
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Italic;
            EditorGUILayout.SelectableLabel($"Serialization component ID: {component.GetID()}", labelStyle, GUILayout.Height(16));

            base.OnInspectorGUI();

            UWheelCollider collider = target as UWheelCollider;

            GUILayout.Label($"Forward slip ratio: {collider.LastTickState.SlipRatio}");
            GUILayout.Label($"Lateral slip angle: {collider.LastTickState.SlipAngle}");
            GUILayout.Label($"Wheel RPM: {collider.wheelState.AngularVelocity * 9.8f}");
            GUILayout.Label($"Rotation angle: {collider.wheelState.RotationAngle}");
            GUILayout.Label($"Longitudinal force: {collider.LastTickState.FrictionForce}");
            //GUILayout.Label($"Suspension force: {collider.LastTickState.SuspensionForce}");
        }
    }
}