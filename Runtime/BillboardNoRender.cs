using UnityEngine;


namespace Peg.Graphics
{
    /// <summary>
    /// Used to orient sprites directly towards the camera so as to
    /// appear to have an orthographic view but with perspective-distance scaling.
    /// 
    /// This version differs frm the normal billboard in that you must specify a transform
    /// that can be used as a face-target. As a result it will not be culled by the rendering
    /// system but will instead update like a normal GameObject.
    /// 
    /// TODO: Add ability to justify a position to an anchor point (top, middle, or bottom)
    /// so that when the camera changes angle it doesn't shift horizontally.
    /// 
    /// - Requires roation offset
    /// 
    /// </summary>
    [AddComponentMenu("Crimson/Sprite/Billboard - No Render")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public partial class BillboardNoRender : AbstractBillboard
    {
        [Tooltip("The object we want our billboard to face.")]
        public Transform FaceTarget;

        public bool AttachToMainCamera = true;


        protected override void OnEnable()
        {
            if(AttachToMainCamera)
                FaceTarget = Camera.main.transform;
        }
        
        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            ProcessBillboard(FaceTarget);
        }
    }
}
