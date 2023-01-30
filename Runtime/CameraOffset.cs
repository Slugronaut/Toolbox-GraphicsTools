using UnityEngine;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Offsets this object based on the current camera viewing angle.
    /// </summary>
    public class CameraOffset : MonoBehaviour
    {
        public float Magnitude;
        public bool FreezeX;
        public bool FreezeY;
        public bool FreezeZ;

        Vector3 Start;
        Transform Trans;
        
        void Awake()
        {
            Trans = transform;
            Start = Trans.localPosition;
        }

        void OnEnable()
        {

        }
        
        void OnWillRenderObject()
        {
            if (!enabled) return;
            var vec = Camera.current.transform.forward * Magnitude;
            float x = FreezeX ? Start.x : Start.x + vec.x;
            float y = FreezeY ? Start.y : Start.y + vec.y;
            float z = FreezeZ ? Start.z : Start.z + vec.z;
            Trans.localPosition = new Vector3(x,y,z);
        }
    }
}
