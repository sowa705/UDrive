using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SimplifiedPacejkaTireData))]
public class SimplifiedPacejkaTireDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AnimationCurve c = new AnimationCurve();
        var tiredata = GetTargetObjectOfProperty(property) as SimplifiedPacejkaTireData;
        float max = 0;
        float maxSlip=0;
        for (int i = 0; i < 80; i++)
        {
            float slip = i / 40f;
            float v = tiredata.CalculateCoF(slip);
            if (v > max)
            {
                max = v;
                maxSlip = slip;
            }
            if (i%2==0)
            {
                c.AddKey(slip, v);
            }
        }
        var a = new Rect(position.x, position.y, position.width, 80);
        var label1 = new Rect(position.x, position.y-30, position.width, 80);
        var label2 = new Rect(position.x, position.y + 30, position.width, 80);

        var unitRect = new Rect(position.x, position.y+90, position.width, position.height-90f);
        EditorGUI.CurveField(a, c,Color.red,new Rect(0,0,2,2));
        EditorGUI.LabelField(label1, $"Peak CoF: {max.ToString("0.000")} ({maxSlip})");
        EditorGUI.LabelField(label2, $"Dynamic CoF: {tiredata.CalculateCoF(3).ToString("0.000")}");
        EditorGUI.PropertyField(unitRect, property, new GUIContent( "Coefficients"), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property)+90f;
    }

    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }
    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }
    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }
}
