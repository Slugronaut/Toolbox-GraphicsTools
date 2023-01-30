using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Allows adjusting of a camera's resolution in realtime using
    /// a rendertarget. Also allows rendering to Gameview in editor
    /// when not in playmode.
    /// </summary>
    [ExecuteInEditMode]
    public class RealtimeCameraResolution : SerializedMonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        Camera _SourceCam;
        [PropertyTooltip("The camera that will use any set RenderTextures.")]
        [ShowInInspector]
        public Camera SourceCam
        {
            get { return _SourceCam; }
            set
            {
                var old = _SourceCam;
                _SourceCam = value;
                if (value != old) UpdateRTState(_Ratio);
            }
        }

        [Tooltip("This is the camera that displays the RenderTexture when scaling the image size.")]
        public Camera RTCamera;

        [Tooltip("If set and rendering at native resolution (1.0 ratio) then we will forego the render texture and simply render directly to the normal camera.")]
        public bool SourceCameraIsNative = true;


        [HideInInspector]
        [SerializeField]
        RawImage _DestImage;
        [PropertyTooltip("The UI element to display the results of the SourceCam rendering.")]
        [ShowInInspector]
        public RawImage DestImage
        {
            get { return _DestImage; }
            set
            {
                var old = _DestImage;
                _DestImage = value;
                if (value != old) UpdateRTState(_Ratio);
            }
        }

        [HideInInspector]
        [SerializeField]
        Canvas _DownsampleCanvas;
        [PropertyTooltip("The canvas used to display the downsampled render texture.")]
        [ShowInInspector]
        public Canvas DownsampleCanvas
        {
            get { return _DownsampleCanvas; }
            set
            {
                var old = _DownsampleCanvas;
                _DownsampleCanvas = value;
                if (value != old) UpdateRTState(_Ratio);
            }
        }


        public RenderTextureFormat Format;
        public int Depth = 32;
        public FilterMode Filter;

        RenderTexture Texture;

        [HideInInspector]
        [SerializeField]
        float _Ratio = 1;
        [PropertyTooltip("The resolution scale of the render target as a scale of the actual rendering resolution.")]
        [ShowInInspector]
        public float Ratio
        {
            get { return _Ratio; }
            set
            {
                if (value > 2) value = 2;
                if (value < 0.01f) value = 0.01f;
                bool noChange = Mathf.Approximately(_Ratio, value);
                _Ratio = value;
                if(!noChange) UpdateRTState(value);
            }
        }

        public bool UsingRT { get { return SourceCam != null && SourceCam.targetTexture != null; } }

        /// <summary>
        /// Disables RT and sets camera to use normal rendering buffer.
        /// </summary>
        void DisableRT()
        {
            if (RTCamera != null) RTCamera.enabled = false;
            if(SourceCam != null) SourceCam.targetTexture = null;
            if (DestImage != null)
            {
                DestImage.enabled = false;
                DestImage.texture = null;
            }
            if (Texture != null && Texture.IsCreated() && Application.isPlaying)
            {
                Texture.Release();
                Destroy(Texture);
            }
            if(_DownsampleCanvas != null)
                _DownsampleCanvas.gameObject.SetActive(false);
        }

        Vector2 ScreenSize
        {
            get
            {
                #if UNITY_EDITOR
                return UnityEditor.Handles.GetMainGameViewSize();
                #else
                return new Vector2(Screen.width, Screen.height);
                #endif
            }
        }

        /// <summary>
        /// Enables use of Rendertexture. If RT was already in use but has changed in ratio, the old one
        /// is released and a new one created.
        /// </summary>
        /// <param name="ratio"></param>
        void EnableRT(float ratio)
        {
            if (ratio < 0.001f) ratio = 0.001f;
            if (!Application.isPlaying)
            {
                DisableRT();
                return;
            }

            //this checks to see if we are already using a render texture and disables it if so.
            //This is so that we can cleanly re-create a new render texture with the desired size.
            if (UsingRT)  DisableRT();

            Vector2 screen = ScreenSize;
            int width = Mathf.RoundToInt(((float)screen.x * ratio));
            int height = Mathf.RoundToInt(((float)screen.y * ratio));
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            
            Texture = new RenderTexture(width, height, Depth, Format);
            Texture.autoGenerateMips = false;
            Texture.filterMode = Filter;
            Texture.name = "Realtime Res Buffer";
            Texture.Create();


            if (RTCamera != null) RTCamera.enabled = true;
            if (SourceCam != null)
            {
                SourceCam.SetTargetBuffers(Texture.colorBuffer, Texture.depthBuffer);
                SourceCam.targetTexture = Texture;
            }
            if (DestImage != null)
            {
                DestImage.enabled = true;
                DestImage.texture = Texture;
            }
            if (_DownsampleCanvas != null)
                _DownsampleCanvas.gameObject.SetActive(true);
        }

        /// <summary>
        /// Determines how to set RT if needed.
        /// </summary>
        /// <param name="ratio"></param>
        void UpdateRTState(float ratio)
        {
            if (!Application.isPlaying || (SourceCameraIsNative && Mathf.Approximately(ratio, 1)))
                DisableRT();
            else EnableRT(ratio);
        }

        void OnEnable()
        {
            UpdateRTState(_Ratio);
        }
    }
}
