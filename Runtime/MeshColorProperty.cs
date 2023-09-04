using UnityEngine;
using Sirenix.OdinInspector;

namespace Peg.Graphics
{
    /// <summary>
    /// Used to set the _Color property of a Material
    /// attached to a MeshRenderer.
    /// </summary>
    [ExecuteInEditMode]
    public class MeshColorProperty : SerializedMonoBehaviour
    {
        public static int ColorId = Shader.PropertyToID("_Color");

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

        protected void Awake()
        {
            if (RendOverride == null) RendOverride = GetComponent<MeshRenderer>();
        }

        protected virtual void Start()
        {
            //this ensures that we apply our serialized field to the property block
            Color = _Color;
        }

        /*
        [InspectorName("Clear Properties")]
        [InspectorButton]
        public void ClearProperties()
        {
            if (RendOverride == null)
                RendOverride = GetComponent<Renderer>();
            RendOverride.GetPropertyBlock(SharedBlock);

            SharedBlock.Clear();
            GetComponent<MeshRenderer>().SetPropertyBlock(SharedBlock);
        }
        */

        [SerializeField]
        protected Color _Color = Color.white;
        [PropertyTooltip("Sets the color for the mesh renderer's _Color property")]
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (RendOverride == null)
                    RendOverride = GetComponent<Renderer>();
                RendOverride.GetPropertyBlock(SharedBlock);

                _Color = value;
                SharedBlock.SetColor(ColorId, value);
                RendOverride.SetPropertyBlock(SharedBlock);
            }
        }

    }
}
