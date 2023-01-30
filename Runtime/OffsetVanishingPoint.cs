using UnityEngine;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Offsets the vanishing point of a perspective camera.
    /// </summary>
    //[ExecuteInEditMode]
    public class OffsetVanishingPoint : MonoBehaviour
    {
        public Camera Cam;
        public float Hor, vert;

        Matrix4x4 DefaultProj;
        Matrix4x4 DefaultWtc;

        void Awake()
        {
            if (Cam == null) Cam = Camera.main;
            DefaultProj = Cam.projectionMatrix;
            DefaultWtc = Cam.projectionMatrix;
        }
        
        Matrix4x4 SetObliqeness(float hor, float vert, Camera cam, Matrix4x4 proj, Matrix4x4 wtc)
        {
            //Apply oblique offset here
            proj[0, 2] = hor;
            proj[1, 2] = vert;
            return proj;
        }

        void OnDisable()
        {
            Cam.projectionMatrix = DefaultProj;
        }

        void Update()
        {
            Apply(Cam, Cam);
        }

        void Apply(Camera source, Camera dest)
        {
            dest.projectionMatrix = SetObliqeness(Hor, vert, source, DefaultProj, DefaultWtc);
        }

    }
}
