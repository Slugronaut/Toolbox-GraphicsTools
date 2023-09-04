using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Peg.Graphics
{
    /// <summary>
    /// For each mesh renderer attached to this hierarchy, it will save
    /// the appropriate lightmap data and resotre it at a later time.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public sealed class LightmapSaveState : MonoBehaviour
    {
        public bool ApplyOnEnable;

        //public string LightmapResourcePath;
        

        void OnEnable()
        {
            if (ApplyOnEnable) RestoreLightmapState();
        }

        /// <summary>
        /// Restores a previous saved lightmap state. Note that the
        /// state can only be saved during edit-time bbut can be restored
        /// both at edit-time and runtime.
        /// </summary>
        public void RestoreLightmapState()
        {
            var states = gameObject.GetComponentsInChildren<LightmapState>(true);
            if (states != null)
            {
                for (int i = 0; i < states.Length; i++)
                    states[i].Restore();
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Can only be used at edit-time. Stores a list of
        /// all MeshRenderes attached to this hierarchy
        /// along wwith their lightmaping info.
        /// </summary>
        public void SaveLightmapData()
        {
            if (Application.isPlaying) return;

            var rends = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            if (rends != null)
            {
                foreach (var rend in rends)
                {
                    var state = rend.GetComponent<LightmapState>();
                    if (state == null) state = rend.gameObject.AddComponent<LightmapState>();
                    state.Save();
                }
            }

            Debug.Log("<color=blue>Saved lightmap state for '" + name + "'.</color>");
            
        }
        #endif

        
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(LightmapSaveState))]
    public class LightmapSaveStateEditor : Editor
    {
        LightmapSaveState Map;

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            Map = target as LightmapSaveState;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (!EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Save Lightmap State"))
                    Map.SaveLightmapData();
            }
            if (GUILayout.Button("Restore Lightmap State"))
                Map.RestoreLightmapState();
            if(GUILayout.Button("Save Meshes"))
            {
                string savePath = EditorUtility.SaveFolderPanel("Dump Meshes To...", "", Map.gameObject.name);
                Debug.Log(savePath);
                /*
                string savePath = Application.dataPath + "/Meshes";
                if (!AssetDatabase.IsValidFolder(savePath))
                    AssetDatabase.CreateFolder("Assets", "Meshes");

                var filters = Map.GetComponentsInChildren<MeshFilter>(true);
                
                foreach (var filter in filters)
                {
                    AssetDatabase.CreateAsset(filter.sharedMesh, "Assets/Meshes/" + filter.name + ".asset");
                }
                AssetDatabase.SaveAssets();
                */
            }
            if (EditorGUI.EndChangeCheck() || GUI.changed)
            {
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


            base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(true);
        }

        
    }
#endif
}
