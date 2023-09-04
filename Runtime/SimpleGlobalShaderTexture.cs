using UnityEngine;
using UnityEngine.SceneManagement;

namespace Peg.Graphics
{
    /// <summary>
    /// Sets a global shader texture property.
    /// </summary>
    [ExecuteInEditMode]
    public class SimpleGlobalShaderTexture : MonoBehaviour
    {
        public string PropertyName;
        public Texture2D Texture;

        static bool Started;


        protected void Awake()
        {
            //need this sentinal due to a bug in the singletons system
            if (!Started)
            {
                Started = true;
                Inject();
                SceneManager.sceneLoaded += HandleLoad;
            }
        }

        protected void OnDestroy()
        {
            if (Started)
            {
                SceneManager.sceneLoaded -= HandleLoad;
            }
        }

        protected void HandleLoad(Scene scene, LoadSceneMode mode)
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
