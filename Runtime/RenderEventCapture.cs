using System;
using Toolbox.AutoCreate;
using UnityEngine;
using UnityEngine.Rendering;


namespace Toolbox.Graphics
{
    /// <summary>
    /// Handles the beginning of each camera's rendering in SRPs. This is the only
    /// way to get access to a camera during the OnWillRenderObject method of monobehaviours.
    /// </summary>
    [AutoCreate]
    public class RenderEventCapture : IDisposable
    {
        public static RenderEventCapture Instance { get; private set; }
        public Camera CurrentCamera { get; private set; }
        bool Disposed = true;

        void AutoAwake()
        {
            Instance = this;
        }

        void AutoStart()
        {
            RenderPipelineManager.beginCameraRendering += HandleBeginCameraRendering;
            Disposed = false; //WARNING: NOT THREAD SAFE!!!!
        }

        void AutoDestroy()
        {
            Dispose();
        }

        void HandleBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            CurrentCamera = camera;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRendering;
            CurrentCamera = null;
            Instance = null;
        }
    }
}

