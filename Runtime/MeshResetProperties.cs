using UnityEngine;
using Sirenix.OdinInspector;

namespace Peg.Graphics
{
    /// <summary>
    /// Resets the Material Property Block of this mesh renderer.
    /// </summary>
    public class MeshResetProperties : SerializedMonoBehaviour
    {

        public Renderer RendOverride;


        protected void Awake()
        {
            if (RendOverride == null) RendOverride = GetComponent<MeshRenderer>();
        }


        [Button("Clear Properties")]
        public void ClearProperties()
        {
            if (RendOverride == null)
                RendOverride = GetComponent<Renderer>();
            RendOverride.GetPropertyBlock(MeshColorProperty.SharedBlock);

            MeshColorProperty.SharedBlock.Clear();
            GetComponent<MeshRenderer>().SetPropertyBlock(MeshColorProperty.SharedBlock);
        }
    }
}
