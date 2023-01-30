using UnityEngine;


namespace Toolbox.Graphics
{
    /// <summary>
    /// Changes a material texture's offsets over time.
    /// </summary>
    [ExecuteInEditMode]
    public class OffsetMaterialTexture : MonoBehaviour
    {
        public Vector2 Offset;
        public Vector2 Speed;
        public Material Mat;
        public string TextureName;

        Vector2 Vec = Vector2.zero;
        int PropId;


        void Start()
        {
            PropId = Shader.PropertyToID(TextureName);
        }

        void Update()
        {
            Mat.SetTextureOffset(PropId, Offset + (Time.time * Speed));
        }
    }
}
