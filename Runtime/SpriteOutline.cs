using UnityEngine;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Attach to a gameobject that has a SpriteRenderer to allow it to display outlines.
    /// This version will update outline properties in realtime.
    /// </summary>
    [AddComponentMenu("Toolbox/Sprite/Sprite Outline - Dynamic")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    [DisallowMultipleComponent]
    public class SpriteOutline : MonoBehaviour, ISpriteOutline
    {
        [SerializeField]
        public Color _Color = Color.white;
        public Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        private SpriteRenderer spriteRenderer;
        MaterialPropertyBlock mpb;
        bool Outline;
        bool LastOutline;
        Color LastColor;

        void Awake()
        {
            mpb = new MaterialPropertyBlock();
        }

        void OnEnable()
        {
            Outline = true;
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateOutline();
        }

        void OnDisable()
        {
            Outline = false;
            UpdateOutline();
        }

        void Update()
        {
            if(LastColor != _Color || LastOutline != Outline)
            { 
                UpdateOutline();
                LastColor = _Color;
                LastOutline = Outline;
            }
        }

        public void UpdateOutline()
        {
            if (mpb == null) return;

            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", Outline ? 1f : 0);
            mpb.SetColor("_OutlineColor", Color);
            spriteRenderer.SetPropertyBlock(mpb);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public interface ISpriteOutline
    {
        Color Color { get; set; }
        void UpdateOutline();
        bool enabled { get; set; }
    }
}