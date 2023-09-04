using UnityEditor;
using Peg.Graphics;

namespace Peg.ToolboxEditor
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(SpriteCollectionParams))]
    [CanEditMultipleObjects]
    public class SpriteCollectionParamsEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var prop = serializedObject.FindProperty("_Alpha");

            serializedObject.Update();
            EditorGUILayout.PropertyField(prop);
            serializedObject.ApplyModifiedProperties();

            foreach(var t in targets)
            {
                var par = t as SpriteCollectionParams;
                par.ForceUpdate();
            }
        }
    }
}
