using UnityEngine;
using UnityEngine.Events;

namespace Peg.Graphics
{
    /// <summary>
    /// Exposes Unity's magic methods OnBecameIvisible and OnBecameVisable as UnityEvents
    /// that can have listeners attached.
    /// </summary>
    public class VisibilityTriggers : MonoBehaviour
    {
        public UnityEvent OnVisible;
        public UnityEvent OnInvisible;

        static bool Quitting;

        public void OnBecameInvisible()
        {
            if(!Quitting)
                OnInvisible.Invoke();
        }

        public void OnBecameVisible()
        {
            if(!Quitting)
                OnVisible.Invoke();
        }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
    }
}
