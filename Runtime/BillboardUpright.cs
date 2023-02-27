using UnityEngine;


namespace Toolbox.Graphics
{
    /// <summary>
    /// Used to orient sprites directly towards the camera so as to
    /// appear to have an orthographic view but with perspective-distance scaling.
    /// 
    /// This version only rotates around the y-Axis. Meaning it won't lean with the camera
    /// if it pitches up or down.
    /// 
    /// TODO: Add ability to justify a position to an anchor point (top, middle, or bottom)
    /// so that when the camera changes angle it doesn't shift horizontally.
    /// 
    /// - Requires roation offset
    /// 
    /// </summary>
    [AddComponentMenu("Crimson/Sprite/Billboard - Upright")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public partial class BillboardUpright : AbstractBillboard
    {
        [Tooltip("The object we want our billboard to face.")]
        public Transform FaceTarget;

        public bool AttachToMainCamera = true;


        protected override void OnEnable()
        {
            if (AttachToMainCamera)
                FaceTarget = Camera.main.transform;
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            ProcessBillboardUpright(FaceTarget);
        }
    }
}
