using UnityEngine;


namespace Toolbox.Graphics
{
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class LightmapState : MonoBehaviour
    {
        public bool ApplyOnEnable;

        public int LightmapIndex;
        public Vector4 ScaleOffset;

        void OnEnable()
        {
            if (ApplyOnEnable) Restore();
        }
        
        public void Save()
        {
            var mr = GetComponent<MeshRenderer>();
            LightmapIndex = mr.lightmapIndex;
            ScaleOffset = mr.lightmapScaleOffset;

        }

        public void Restore()
        {
            var mr = GetComponent<MeshRenderer>();
            mr.lightmapIndex = LightmapIndex;
            mr.lightmapScaleOffset = ScaleOffset;
        }
    }
}
