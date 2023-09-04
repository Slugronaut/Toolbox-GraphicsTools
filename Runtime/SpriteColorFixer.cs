using UnityEngine;

namespace Peg.Graphics
{
    /// <summary>
    /// This is a workaround for Unity 5.6b9 for setting the
    /// a sprite's vertex color at startup.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteColorFixer : MonoBehaviour
    {
        SpriteRenderer Sr;

        static MaterialPropertyBlock Block;
        static bool BlockExists;
        const string Prop = "_Color";

        void Start()
        {
            Sr = GetComponent<SpriteRenderer>();

            //un-comment this section once Unity has its color bug fixed and all instances of this script in a scene will disappear
            //if (!Application.isPlaying) DestroyImmediate(this);
        }
        

        void Update()
        {
            //can't use static contructor due to way unity initializes things
            if (!BlockExists)
            {
                Block = new MaterialPropertyBlock();
                BlockExists = true;
            }
            
            Block.SetColor(Prop, Sr.color);
            Sr.SetPropertyBlock(Block);
        }
    }
}
