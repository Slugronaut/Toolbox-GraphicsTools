using Peg.Lib;
using UnityEngine;
using UnityEngine.Events;

namespace Peg.Graphics
{
    /// <summary>
    /// Invokes listeners whenever this object's center leaves the camera's viewport.
    /// </summary>
    public class ViewportTriggers : MonoBehaviour
    {
        public enum ViewportFlags
        {
            Visible = 1,
            JustSpawned = 1 << 1,
        }

        public float xSafeZone;
        public float ySafeZone;

        public UnityEvent OnVisible;
        public UnityEvent OnInvisible;

        byte Flags;
        Transform Trans;
        static Camera MainCam;


        public bool IsVisible
        {
            get { return (Flags & (byte)ViewportFlags.Visible) != 0; }
            set
            {
                if (value) Flags |= (byte)ViewportFlags.Visible;
                else Flags &= ((byte)ViewportFlags.Visible ^ 0xff);
            }
        }

        public bool JustSpawned
        {
            get { return (Flags & (byte)ViewportFlags.JustSpawned) != 0; }
            set
            {
                if (value) Flags |= (byte)ViewportFlags.JustSpawned;
                else Flags &= ((byte)ViewportFlags.JustSpawned ^ 0xff);
            }
        }

        private void Awake()
        {
            MainCam = Camera.main;
            Trans = transform;
        }

        private void OnEnable()
        {
            if (MainCam == null || Peg.TypeHelper.IsReferenceNull(MainCam))
                MainCam = Camera.main;
            JustSpawned = true;
        }

        public void LateUpdate()
        {
            if (JustSpawned)
                JustSpawned = false;
            else CheckViewport();
        }

        public void CheckViewport()
        {
            bool state = MathUtils.IsInViewport(MainCam, Trans.position, xSafeZone, ySafeZone);

            if(state != IsVisible)
            {
                IsVisible = state;
                if (state) OnVisible.Invoke();
                else OnInvisible.Invoke();
            }
            
        }
    }
}
