using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crimson
{
    /// <summary>
    /// Fakes a billboard effect that would rotate on the x-axis
    /// by instead, scaling an object on the y-axis and then
    /// shifting up to keep vertical positioning.
    /// </summary>
    [AddComponentMenu("Crimson/Billboard - Projected")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public partial class BillboardProjected : MonoBehaviour
    {
        [Tooltip("If set, this object will only update once and then disable itself to save on CPU.")]
        public bool UpdateOnlyOnEnable = true;

        [Tooltip("Should this object rotate around its y-axis to face the camera?")]
        public bool RotateOnY = true;

        //Cached for performance.
        Transform Trans;


        void Awake()
        {
#if UNITY_5_5_OR_NEWER
            SceneManager.sceneLoaded += OnSceneLoaded;
#endif
            Trans = transform;
        }

        void OnEnable()
        {
            //this is here just so we have the ability to disable this behaviour in the inspector
        }

#if UNITY_5_5_OR_NEWER
        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(UpdateOnlyOnEnable) enabled = true;
        }
        #else
        void OnLevelWasLoaded(int i)
        {
            enabled = true;
        }
        #endif

        /// <summary>
        /// 
        /// </summary>
        void OnWillRenderObject()
        {
            if (!isActiveAndEnabled) return;
            UpdateView(Camera.current.transform);

            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (UpdateOnlyOnEnable) this.enabled = false;
            }
            #else
            if (UpdateOnlyOnEnable) this.enabled = false;
            #endif

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        void UpdateView(Transform target)
        {
            float camAngle = Vector3.Angle(Vector3.up, target.forward);
            float scale = 1.0f / Mathf.Sin((camAngle) * Mathf.PI / 180);
            transform.localScale = new Vector3(1, scale, 1);

            if (RotateOnY)
            {
                Vector3 dir = target.forward;
                Trans.rotation = Quaternion.LookRotation(dir);
                var rot = Trans.localEulerAngles;
                Trans.localEulerAngles = new Vector3(0, rot.y, rot.z);
            }
        }
        
    }
}
