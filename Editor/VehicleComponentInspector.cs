using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]

[CustomEditor(typeof(VehicleComponent),true)]
public class VehicleComponentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        VehicleComponent component = target as VehicleComponent;
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Italic;
        if (component is IStatefulComponent)
        {
            EditorGUILayout.SelectableLabel($"Serialization component ID: {component.GetID()}", labelStyle, GUILayout.Height(16));

        }
        base.OnInspectorGUI();
    }
}