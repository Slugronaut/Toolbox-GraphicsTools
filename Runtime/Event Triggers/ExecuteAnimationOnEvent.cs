using UnityEngine.Events;


namespace Toolbox.Graphics
{
    public class ExecuteAnimationOnEvent : AbstractOperationOnEvent
    {
        public AnimatorEx Animator;
        public HashedString AnimName;
        public bool IsFixedTime;
        public float PlayTime;
        public AnimatorEx.PlayMode Mode;
        public int Layer;

        public UnityEvent OnAnimComplete = new UnityEvent();


        public override void PerformOp()
        {
            Animator.ExecuteAnimation(AnimName.Hash, Layer, PlayTime, IsFixedTime, Mode, HandleAnimComplete);
        }

        void HandleAnimComplete()
        {
            OnAnimComplete.Invoke();
        }

    }
}
