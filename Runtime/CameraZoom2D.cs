using UnityEngine;
using System.Collections;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Attach to a camera to allow it to be zoomed using the middle mouse button.
    /// TODO: add support for touchscreen pinches.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraZoom2D : MonoBehaviour
    {
        public bool DefaultIsStartingSize = false;
        public float DefaultSize = 7;
        public float Speed = 1;
        public float Max = 12;
        public float Min = 4;
        public bool Reverse;
        public int ResetButton = 2;
        Camera Cam;


        void Awake()
        {
            Init();
        }

        void Update()
        {
            if(Cam == null) Init();
            if (Cam != null)
            {
                if (Cam.orthographic)
                {
                    Cam.orthographicSize += Input.mouseScrollDelta.y * Speed * ((Reverse) ? -1.0f : 1.0f);
                    if (Cam.orthographicSize < Min) Cam.orthographicSize = Min;
                    else if (Cam.orthographicSize > Max) Cam.orthographicSize = Max;

                    if (Input.GetMouseButtonDown(ResetButton)) Cam.orthographicSize = DefaultSize;
                }
                else
                {
                    Vector3 pos = Cam.transform.localPosition;
                    float newZ = pos.z += Input.mouseScrollDelta.y * Speed * ((!Reverse) ? -1.0f : 1.0f);
                    if (newZ < -Max) newZ = -Max;
                    else if (newZ > Min) newZ = Min;
                    if (Input.GetMouseButtonDown(ResetButton)) newZ = -DefaultSize;
                    Cam.transform.localPosition = new Vector3(pos.x, pos.y, newZ);
                    
                }
            }
        }

        void Init()
        {
            Cam = GetComponent<Camera>();
            if (Cam != null)
            {
                if (DefaultIsStartingSize)
                {
                    if (Cam.orthographic) DefaultSize = Cam.orthographicSize;
                }
            }
            else this.enabled = false;
        }

    }
}
