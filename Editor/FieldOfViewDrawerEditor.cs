using UnityEngine;
using UnityEditor;
using System;
using Peg.Graphics;

namespace Peg.ToolboxEditor
{
    /// <summary>
    /// Necessary because FieldOfViewDrawer uses properties extensively to internally manage
    /// certain states when accessing what would otherwise be public fields.
    /// </summary>
    [CustomEditor(typeof(FieldOfViewDrawer))]
    public class FieldOfViewDrawerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FieldOfViewDrawer drawer = target as FieldOfViewDrawer;

            EditorGUI.BeginChangeCheck();

            drawer.Angle = EditorGUILayout.FloatField("Angle", drawer.Angle);
            drawer.MinDist = EditorGUILayout.FloatField("Min Dist", drawer.MinDist);
            drawer.MaxDist = EditorGUILayout.FloatField("Max Dist", drawer.MaxDist);
            drawer.Alignment = (FieldOfViewDrawer.Orientation)EditorGUILayout.EnumPopup("Orientation", (Enum)drawer.Alignment);
            drawer.material = EditorGUILayout.ObjectField("Material", drawer.material, typeof(Material), false) as Material;
            int i  = EditorGUILayout.IntField("Layer", drawer.Layer);
            if (i < 0) i = 0;
            else if (i > 31) i = 31;
            drawer.Layer = i;
            if(EditorGUI.EndChangeCheck() || GUI.changed) EditorUtility.SetDirty(target);
        }
        
    }
}
