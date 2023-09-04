using System;
using Peg.Messaging;
using UnityEngine.Events;


namespace Peg.Graphics
{
    public class ExecuteAnimationOnMessage : AbstractMessageReciever
    {
        public AnimatorEx Animator;
        public HashedString AnimName;
        public bool IsFixedTime;
        public float PlayTime;
        public AnimatorEx.PlayMode Mode;
        public int Layer;

        public UnityEvent OnAnimComplete = new UnityEvent();


        protected override void HandleMessage(Type msgType, object msg)
        {
            Animator.ExecuteAnimation(AnimName.Hash, Layer, PlayTime, IsFixedTime, Mode, HandleAnimComplete);
        }

        void HandleAnimComplete()
        {
            OnAnimComplete.Invoke();
        }

    }
}
