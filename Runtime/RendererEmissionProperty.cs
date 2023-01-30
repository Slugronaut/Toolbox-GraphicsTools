using UnityEngine;


namespace Toolbox.Graphics
{
    /// <summary>
    /// Used to set the _EmissionColor property of a renderer's material.
    /// </summary>
    [ExecuteInEditMode]
    public class RendererEmissionProperty : MonoBehaviour
    {
        public static int ColorId = Shader.PropertyToID("_EmissionColor");
        static MaterialPropertyBlock _SharedBlock;
        public static MaterialPropertyBlock SharedBlock
        {
            get
            {
                if (_SharedBlock == null)
                    _SharedBlock = new MaterialPropertyBlock();
                return _SharedBlock;
            }
        }

        public Renderer RendOverride;
        [ColorUsage(true, true)]
        public Color Color = Color.white;
        Color LastColor;


        void OnValidate()
        {
            Update();
        }

        protected void Awake()
        {
            if (RendOverride == null) RendOverride = GetComponent<MeshRenderer>();
        }

        public void Update()
        {
            if(LastColor != Color)
            {
                if (RendOverride == null) return;
                RendOverride.GetPropertyBlock(SharedBlock);
                SharedBlock.SetColor(ColorId, Color);
                RendOverride.SetPropertyBlock(SharedBlock);
                LastColor = Color;
            }
        }

    }
}
