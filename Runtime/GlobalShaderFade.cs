using UnityEngine;


namespace Peg.Graphics
{
    /// <summary>
    /// Sets the global shader variable '_ReferencePoint' to this gameobject's
    /// current poisition so that fading materials work properly.
    /// </summary>
    [ExecuteInEditMode]
    public class GlobalShaderFade : MonoBehaviour
    {
        public const string Shader_ReferencePoint = "_ReferencePoint";
        public const string ShaderKeyword_DITHER_VERTICAL_FADE = "DITHER_VERTICAL_FADE";
        Transform Trans;

        void Awake()
        {
            Trans = transform;
        }

        void OnEnable()
        {
            Shader.EnableKeyword(ShaderKeyword_DITHER_VERTICAL_FADE);
        }

        void OnDisable()
        {
            Shader.DisableKeyword(ShaderKeyword_DITHER_VERTICAL_FADE);
        }

        void OnDestroy()
        {
            OnDisable();
        }

        void OnApplicationQuit()
        {
            Shader.DisableKeyword(ShaderKeyword_DITHER_VERTICAL_FADE);
        }

        void Update()
        {
            Shader.SetGlobalVector(Shader_ReferencePoint, Trans.position);
        }
    }
}