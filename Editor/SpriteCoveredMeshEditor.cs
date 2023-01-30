#if CRIMSON
using Toolbox.Extensions.Crimson;
using UnityEditor;
using UnityEngine;

namespace Toolbox.ToolboxEditor
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(SpriteCoveredMesh))]
    [CanEditMultipleObjects]
    public class SpriteCoveredMeshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);
            if (GUILayout.Button("Cover Mesh"))
            {
                for(int i = 0; i < targets.Length; i++)
                {
                    var scm = targets[i] as SpriteCoveredMesh;
                    if (scm != null)
                    {
                        scm.Initialized = false;
                        scm.Init();
                    }
                    EditorUtility.DisplayProgressBar("Processing", "Please wait...", (float)i / (float)targets.Length);
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
#endif