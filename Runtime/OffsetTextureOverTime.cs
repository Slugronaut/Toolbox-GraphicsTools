using UnityEngine;


namespace Toolbox.Graphics
{
    /// <summary>
    /// Changes a texture's offsets over time.
    /// </summary>
    public class OffsetTextureOverTime : MonoBehaviour
    {
        public float SpeedX;
        public float SpeedY;
        public Renderer Renderer;
        public string Property;

        Vector2 Vec = Vector2.zero;
        int PropId;
        static MaterialPropertyBlock Block;
        

        void Start()
        {
            if(Block == null)
                Block = new MaterialPropertyBlock();

            PropId = Shader.PropertyToID(Property);
        }

        void Update()
        {
            var d = Time.deltaTime;
            Renderer.GetPropertyBlock(Block);
            var vec = Block.GetVector(PropId);
            vec.x = 1;
            vec.y = 1;//have to set these to 1 for some stupid reason :(
            vec.z += SpeedX * d;
            vec.w += SpeedY * d;
            Block.SetVector(PropId, vec);
            Renderer.SetPropertyBlock(Block);
        }
    }
}
