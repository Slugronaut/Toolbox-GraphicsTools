using UnityEngine;


namespace Peg.Graphics
{
    /// <summary>
    /// Sync the normalized between two named states when transitioning between them.
    /// </summary>
    [System.Obsolete("Failed experiment")]
    [RequireComponent(typeof(Animator))]
    public class SyncAnimStates : MonoBehaviour
    {
        public HashedString State1;
        public HashedString State2;
        public int LayerIndex = -1;

        Animator Anim;
        AnimatorStateInfo LastState;



        void Awake()
        {
            Anim = GetComponent<Animator>();
        }

        void Update()
        {
            ////float time = 0;
            //if(Anim.IsInTransition(LayerIndex))
            //{
                //var trans = Anim.GetAnimatorTransitionInfo(LayerIndex);
                //time = trans.normalizedTime;
            //}

            //Crimson.AnimatorActionControl ac;
            //Crimson.AnimatorActionControl.
            var info = Anim.GetCurrentAnimatorStateInfo(LayerIndex);
            if(info.fullPathHash != LastState.fullPathHash)
            {
                if (info.fullPathHash == State1.Hash || info.fullPathHash == State2.Hash)
                    Anim.Play(info.fullPathHash, LayerIndex, info.normalizedTime);

                LastState = info;
            }
        }
    }
}
