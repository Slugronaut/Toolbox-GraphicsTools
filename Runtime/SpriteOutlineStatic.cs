#if CRIMSON
using UnityEngine;

namespace Toolbox.Extensions.Crimson
{
    /// <summary>
    /// Attach to a gameobject that has a SpriteRenderer to allow it to display outlines.
    /// This version will only update the outline properties when enabled or disabled.
    /// </summary>
    [AddComponentMenu("Crimson/Sprite/Sprite Outline - Static")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    [DisallowMultipleComponent]
    public class SpriteOutlineStatic : MonoBehaviour, ISpriteOutline
    {
        [SerializeField]
        public Color _Color = Color.white;
        public Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        private SpriteRenderer spriteRenderer;


        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateOutline(true);
        }

        void OnDisable()
        {
            UpdateOutline(false);
        }

        void UpdateOutline(bool outline)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", outline ? 1f : 0);
            mpb.SetColor("_OutlineColor", Color);
            spriteRenderer.SetPropertyBlock(mpb);
        }

        public void UpdateOutline()
        {
            UpdateOutline(enabled);
        }
    }
}
#endif