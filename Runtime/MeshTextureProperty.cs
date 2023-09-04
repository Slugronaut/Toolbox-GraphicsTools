using UnityEngine;
using Sirenix.OdinInspector;

namespace Peg.Graphics
{
    /// <summary>
    /// Used to set the _MainTex property of a Material
    /// attached to a MeshRenderer.
    /// </summary>
    [ExecuteInEditMode]
    public class MeshTextureProperty : MeshColorProperty
    {
        public static int TexId = Shader.PropertyToID("_MainTex");
        
        
        protected override void Start()
        {
            base.Start();
            //this ensures that we apply our serialized field to the property block
            Texture = _Texture;
        }

        [SerializeField]
        protected Texture _Texture;
        [PropertyTooltip("Sets the texture for the mesh renderer's _MainTex property")]
        [ShowInInspector]
        public Texture Texture
        {
            get { return _Texture; }
            set
            {
                if (RendOverride == null)
                    RendOverride = GetComponent<Renderer>();
                RendOverride.GetPropertyBlock(SharedBlock);


                _Texture = value;
                if (_Texture != null)
                    SharedBlock.SetTexture(TexId, _Texture);
                else
                {
                    SharedBlock.Clear();
                    SharedBlock.SetColor(ColorId, _Color);
                }
                GetComponent<MeshRenderer>().SetPropertyBlock(SharedBlock);
            }
        }

        

    }
}
