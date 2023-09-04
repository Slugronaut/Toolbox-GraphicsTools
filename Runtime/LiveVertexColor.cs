using UnityEngine;
using Sirenix.OdinInspector;

namespace Peg.Graphics
{
    /// <summary>
    /// Used to change a mesh's per-renderer vertex color tint.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class LiveVertexColor : SerializedMonoBehaviour
    {
        [Tooltip("The name of the material's property that will be set.")]
        public string PropertyName = "_Color";

        MaterialPropertyBlock Block;

        [SerializeField]
        [HideInInspector]
        Color _Tint;

        [ShowInInspector]
        public Color Tint
        {
            get { return _Tint; }
            set
            {
                if (isActiveAndEnabled)
                {
                    if (Block == null) Block = new MaterialPropertyBlock();
                    _Tint = value;
                    Block.SetColor(PropertyName, _Tint);
                    GetComponent<MeshRenderer>().SetPropertyBlock(Block);
                }
            }
        }

        void Start()
        {
            Tint = _Tint;
        }

    }
}
