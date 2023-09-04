using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

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
    [AddComponentMenu("Crimson/Sprite/Billboard (Advanced)")]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public partial class BillboardAdvanced : AbstractBillboard
    {
        [Flags]
        public enum Flags : sbyte
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2,
            XRot = 1 << 3,
            YRot = 1 << 4,
            ZRot = 1 << 5,
            Cull = 1 << 6,
        }

        [HideInInspector]
        [SerializeField]
        sbyte Sync;


        [PropertyTooltip("Update X offset rotation.")]
        [ShowInInspector]
        public bool UpdateXOffset
        {
            get { return (Sync & (sbyte)Flags.XRot) > 0; }
            set
            {
                if (value)
                    Sync |= (sbyte)Flags.XRot;
                else Sync &= ~(sbyte)Flags.XRot;
            }
        }

        [PropertyTooltip("Update Y offset rotation.")]
        [ShowInInspector]
        public bool UpdateYOffset
        {
            get { return (Sync & (sbyte)Flags.YRot) > 0; }
            set
            {
                if (value)
                    Sync |= (sbyte)Flags.YRot;
                else Sync &= ~(sbyte)Flags.YRot;
            }
        }

        [PropertyTooltip("Update Z offset rotation.")]
        [ShowInInspector]
        public bool UpdateZOffset
        {
            get { return (Sync & (sbyte)Flags.ZRot) > 0; }
            set
            {
                if (value)
                    Sync |= (sbyte)Flags.ZRot;
                else Sync &= ~(sbyte)Flags.ZRot;
            }
        }

        [PropertyTooltip("Update X rotation.")]
        [ShowInInspector]
        public bool UpdateXRot
        {
            get { return (Sync & (sbyte)Flags.X) > 0; }
            set
            {
                if (value)
                    Sync |= (sbyte)Flags.X;
                else Sync &= ~(sbyte)Flags.X;
            }
        }

        [PropertyTooltip("Update Y rotation.")]
        [ShowInInspector]
        public bool UpdateYRot
        {
            get { return (Sync & (sbyte)Flags.Y) > 0; }
            set
            {
                if (value)
                    Sync |= (sbyte)Flags.Y;
                else Sync &= ~(sbyte)Flags.Y;
            }
        }

        [PropertyTooltip("Update Z rotation.")]
        [ShowInInspector]
        public bool UpdateZRot
        {
            get { return (Sync & (sbyte)Flags.Z) > 0; }
            set
            {
                if (value)
                    Sync |= (sbyte)Flags.Z;
                else Sync &= ~(sbyte)Flags.Z;
            }
        }

        public bool Culled
        {
            get => (Sync & (sbyte)Flags.Cull) > 0;
            private set
            {
                if (value)
                    Sync |= (sbyte)Flags.Cull;
                else Sync &= ~(sbyte)Flags.Cull;
            }
        }


        #if UNITY_EDITOR
        //these are used in the editor to help alieviate lag when rotating the camera in the Scene view window
        int EditorSkip = 0;
        const int EditorSkipMax = 10;

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorSkip = UnityEngine.Random.Range(0, EditorSkipMax);
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

            ProcessBillboard(RenderEventCapture.Instance.CurrentCamera.transform, false, UpdateXOffset, UpdateYOffset, UpdateZOffset, UpdateXRot, UpdateYRot, UpdateZRot);
        }
    }


    
}
