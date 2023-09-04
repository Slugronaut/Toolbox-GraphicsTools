using UnityEngine;


namespace Peg.Graphics
{
    [ExecuteInEditMode]
    public class SetGlobalTexture : MonoBehaviour
    {
        public string PropertyName;
        public Texture2D Texture;
        
        void OnEnable()
        {
            if(!string.IsNullOrEmpty(PropertyName) && Texture != null)
                Shader.SetGlobalTexture(PropertyName, Texture);
        }
    }
}
