using UnityEngine;
using UnityEngine.SceneManagement;


namespace Peg.Graphics
{
    /// <summary>
    /// Used to orient sprites directly towards the camera so as to
    /// appear to have an orthographic view but with perspective-distance scaling.
    /// 
    /// TODO: Add ability to justify a position to an anchor point (top, middle, or bottom)
    /// so that when the camera changes angle it doesn't shift horizontally.
    /// 
    /// 
    /// </summary>
    [AddComponentMenu("Crimson/Sprite/Billboard")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public partial class Billboard : AbstractBillboard
    {
        #if UNITY_EDITOR
        //these are used in the editor to help alieviate lag when rotating the camera in the Scene view window
        int EditorSkip = 0;
        const int EditorSkipMax = 10;

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorSkip = Random.Range(0, EditorSkipMax);
        }
        #endif


        void OnWillRenderObject()
        {
            if (RenderEventCapture.Instance == null) return;
            #if UNITY_EDITOR
            if (!Application.isPlaying && Application.isEditor)
            {
                if (EditorSkip < EditorSkipMax)
                {
                    EditorSkip++;
                    return;
                }
            }
            EditorSkip = 0;
            #endif

            ProcessBillboard(RenderEventCapture.Instance.CurrentCamera.transform);
        }
    }



    /// <summary>
    /// Base class for all billboard components used by Crimson.
    /// </summary>
    public abstract partial class AbstractBillboard : MonoBehaviour
    {
        [Tooltip("If true, will apply rotation in the transform's local space.")]
        public bool LocalRot;

        [Tooltip("If set, this object will only update once and then disable itself to save on CPU.")]
        public bool UpdateOnlyOnEnable = false;

        [Tooltip("A localized positional offset that is defined in local coordinates of the billboard.")]
        public Vector3 PositionOffset;

        [Tooltip("An offset rotation that can be applied after the billboard roation.")]
        public Vector3 AngleOffset;

        [Tooltip("Used to skip a number of frames in a row in order to save on processing time. Best if 0 or 1. If there is more than one camera rendering the same billboard it is recommened that FrameSkip be set to 0.")]
        public int FrameSkip;
        int Counter;

        //Cached for performance.
        Transform Trans;

        //house-keeping stuff - mostly used to inegrate with other libraries that need to define partial derived classes.
        protected virtual void PartialAwake() { }
        protected virtual void PartialDestroy() { }

        void Awake()
        {
            #if UNITY_5_5_OR_NEWER
            SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
            Trans = transform;
            //randomize the frameskip so that if we have many billboards spawn all at once
            //they don't all hit us with an expensive rotation at the same time, every interval. 
            Counter = Random.Range(0, FrameSkip + 1);
            PartialAwake();
        }

        protected virtual void OnEnable()
        {
            //this is here just so we have the ability to disable this behaviour in the inspector
            Counter = 0;
        }

        #if UNITY_5_5_OR_NEWER
        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            PartialDestroy();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (UpdateOnlyOnEnable) enabled = true;
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
        /// <param name="faceTarget"></param>
        /// <param name="forceUpdate"></param>
        protected void ProcessBillboard(Transform faceTarget, bool forceUpdate, bool xOffset, bool yOffset, bool zOffset, bool xroto, bool yroto, bool zroto)
        {
            if (!isActiveAndEnabled && !forceUpdate) return;

            //NOTE: If there is more than one camera rendering, Frameskip won't work so hot!
            if (Counter < FrameSkip)// && Time.deltaTime < CrimsonUtility.FourtyFifthFrameTime)
            {
                Counter++;
                return;
            }

            Counter = 0;
            UpdateView(faceTarget, xOffset, yOffset, zOffset, xroto, yroto, zroto);

            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (UpdateOnlyOnEnable) enabled = false;
            }
            #else
            if (UpdateOnlyOnEnable)enabled = false;
            #endif
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ProcessBillboard(Transform faceTarget, bool forceUpdate = false)
        {
            if (!isActiveAndEnabled && !forceUpdate) return;

            //NOTE: If there is more than one camera rendering, Frameskip won't work so hot!
            if (Counter < FrameSkip)// && Time.deltaTime < CrimsonUtility.FourtyFifthFrameTime)
            {
                Counter++;
                return;
            }

            Counter = 0;
            UpdateView(faceTarget);

            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (UpdateOnlyOnEnable) enabled = false;
            }
            #else
            if (UpdateOnlyOnEnable)enabled = false;
            #endif
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ProcessBillboardUpright(Transform faceTarget, bool forceUpdate = false)
        {
            if (!isActiveAndEnabled && !forceUpdate) return;

            //NOTE: If there is more than one camera rendering, Frameskip won't work so hot!
            if (Counter < FrameSkip)// && Time.deltaTime < CrimsonUtility.FourtyFifthFrameTime)
            {
                Counter++;
                return;
            }

            Counter = 0;
            UpdateViewUpright(faceTarget);

            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (UpdateOnlyOnEnable) enabled = false;
            }
            #else
            if (UpdateOnlyOnEnable)enabled = false;
            #endif
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateView(Transform target)
        {
            if (target == null) return;
            Vector3 dir = target.forward;
            if (LocalRot) Trans.localRotation = Quaternion.LookRotation(dir);
            else Trans.rotation = Quaternion.LookRotation(dir);
            
            if (AngleOffset != Vector3.zero)
            {
                var eangles = Trans.eulerAngles;
                eangles.x += AngleOffset.x;
                eangles.y += AngleOffset.y;
                eangles.z += AngleOffset.z;
                Trans.eulerAngles = eangles;
            }

            if(PositionOffset != Vector3.zero)
            {
                Trans.localPosition = Trans.InverseTransformDirection(Trans.up) + PositionOffset;
            }

            //TODO: profile to see which is faster
            //
            //  Trans.rotation = Quaternion.LookRotation(dir);
            //
            //  or
            //
            //  Trans.LookAt(dir);
            //  Trans.Rotate(0, 180, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateViewUpright(Transform target)
        {
            if (target == null) return;
            Vector3 dir = target.forward;
            if (LocalRot) Trans.localRotation = Quaternion.LookRotation(dir);
            else Trans.rotation = Quaternion.LookRotation(dir);

            var eangles = Trans.eulerAngles;
            eangles.x = 0;
            eangles.z = 0;
            Trans.eulerAngles = eangles;

            if (PositionOffset != Vector3.zero)
            {
                Trans.localPosition = Trans.InverseTransformDirection(Trans.up) + PositionOffset;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        protected void UpdateView(Transform target, bool x, bool y, bool z, bool xroto, bool yroto, bool zroto)
        {
            if (target == null) return;
            Vector3 dir = target.forward;

            if (LocalRot)
            {
                var oldE = Trans.localRotation.eulerAngles;
                var lookE = Quaternion.LookRotation(dir).eulerAngles;
                Trans.localRotation = Quaternion.Euler(new Vector3(xroto ? lookE.x : oldE.x, yroto ? lookE.y : oldE.y, zroto ? lookE.z : oldE.z));
            }
            else
            {
                var oldE = Trans.rotation.eulerAngles;
                var lookE = Quaternion.LookRotation(dir).eulerAngles;
                Trans.rotation = Quaternion.Euler(new Vector3(xroto ? lookE.x : oldE.x, yroto ? lookE.y : oldE.y, zroto ? lookE.z : oldE.z));
            }

            if (AngleOffset != Vector3.zero)
            {
                var eangles = Trans.eulerAngles;
                if (xroto)
                    eangles.x += AngleOffset.x;
                if (yroto)
                    eangles.y += AngleOffset.y;
                if (zroto)
                    eangles.z += AngleOffset.z;

                Trans.eulerAngles = eangles;
            }

            if (PositionOffset != Vector3.zero)
            {
                var oldPos = Trans.localPosition;
                Trans.localPosition = Trans.InverseTransformDirection(Trans.up) + new Vector3(x ? PositionOffset.x : oldPos.x,
                                                                                              z ? PositionOffset.y : oldPos.y,
                                                                                              x ? PositionOffset.z : oldPos.z);
            }
        }
    }
}
