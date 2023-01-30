using Toolbox.AutoCreate;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Sets a global shader texture property.
    /// </summary>
    //[ExecuteInEditMode]
    [AutoCreate(CreationActions.DeserializeSingletonData)]
    public sealed class SetGlobalShaderTexture
    {
        public string PropertyName;
        public Texture2D Texture;

        static bool Started;

        
        void AutoAwake()
        {
            //need this sentinal due to a bug in the singletons system
            if(!Started)
            {
                Started = true;
                Inject();
                SceneManager.sceneLoaded += HandleLoad;
            }
        }

        void AutoDestroy()
        {
            if(Started)
            {
                SceneManager.sceneLoaded -= HandleLoad;
            }
        }

        void HandleLoad(Scene scene, LoadSceneMode mode)
        {
            //need to due this on scene load because the built-in shaders get reset when scenes load.
            Inject();
        }

        public void Inject()
        {
            if (!string.IsNullOrEmpty(PropertyName) && Texture != null)
                Shader.SetGlobalTexture(PropertyName, Texture);
        }

    }
}
