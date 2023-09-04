using UnityEngine;
using UnityEngine.Events;


namespace Peg.Graphics
{
    /// <summary>
    /// Handler for executing animations that is meant to be called by other objects or linked to UnityEvents.
    /// </summary>
    public class ExecuteAnimationHandler : MonoBehaviour
    {
        public AnimatorEx[] Animators;
        public HashedString AnimName;
        public bool IsFixedTime;
        public float PlayTime;
        public AnimatorEx.PlayMode Mode;
        public int Layer;

        [Tooltip("This event will only be triggered by the firt animation in the array.")]
        public UnityEvent OnAnimComplete = new UnityEvent();


        public void Execute()
        {
            if(Animators.Length > 0)
                Animators[0].ExecuteAnimation(AnimName.Hash, Layer, PlayTime, IsFixedTime, Mode, HandleAnimComplete);

            for(int i = 1; i < Animators.Length; i++)
                Animators[i].ExecuteAnimation(AnimName.Hash, Layer, PlayTime, IsFixedTime, Mode, HandleAnimComplete);
        }

        void HandleAnimComplete()
        {
            OnAnimComplete.Invoke();
        }

    }
}
