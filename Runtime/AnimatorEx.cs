using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Peg.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorEx : MonoBehaviour
    {
        public enum NameHashTypes
        {
            Short,
            Long,
            Legacy,
        }
        /// <summary>
        /// 
        /// </summary>
        public struct AnimExecution
        {
            public float Time;
            public bool FixedTime;
            public int Layer;
            public int StateNameHash;
            public UnityAction Callback;

            public AnimExecution(float time, bool fixedTime, int layer, int stateNameHash, UnityAction callback)
            {
                Time = time;
                FixedTime = fixedTime;
                Layer = layer;
                StateNameHash = stateNameHash;
                Callback = callback;
            }
        }

        static readonly float  MinStateTime = 0.01f;
        
        [Tooltip("Easy access to controlling the overal speed of the animator playback using this component.")]
        public float SpeedScale = 1;
        [Tooltip("Should we scale the animator's root speed to change the timing of specific animations?")]
        [SuffixLabel("(Experimental)")]
        public bool UseAnimatorSpeed = true;
        [Tooltip("The parameter that controls playback speed of clips. This is only used if we are not using the animator's root speed.")]
        [Indent(1)]
        [HideIf("UseAnimatorSpeed")]
        public HashedString PlaybackSpeedParam = new HashedString("PlaybackSpeed");
        [Tooltip("Mechanim is a right, and truely, fucked pile of fuck-sticks with bits of poo dangling from single strands of hair off of them. In some cases you'll want to use substates, in which case, all animations *must* supply the layer name. In such a case, the 'end-of-animation' test won't work unless this is ticked. Good fucking luck you poor bastard should you choose to use nested states. Potentially you can get away with using the legacy mode but since it's useful they've marked it as depreciated and will likely remove it once people realize how badly they need it. Fuck...")]
        public NameHashTypes NameHashType;

        Queue<AnimExecution> AnimQueue = new Queue<AnimExecution>(5);
        Animator StateMachine;
        UnityAction Callback;
        bool StateChangePending;
        int CurrentStatePending;

        /// <summary>
        /// Sets the speed of the internal AnimatorController's statemachine.
        /// </summary>
        /// <param name="val"></param>
        public float FSMSpeed
        {
            get { return StateMachine.speed; }
            set { if (StateMachine.isInitialized) StateMachine.speed = value; }
        }

        public Animator BackingAnimator
        {
            get => StateMachine;
        }


        #region Unity Events
        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        {
            StateMachine = GetComponent<Animator>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnEnable()
        {
            //need to do this because Unity seems to loose track of the reference when enabling/disabling
            StateMachine = GetComponent<Animator>();
        }

        /// <summary>
        /// Polls the current animation state and informs us when it changes
        /// </summary>
        void Update()
        {
            
            if (StateChangePending)
            {
                bool isSameState = IsCurrentState(0, CurrentStatePending);
                //POTENTIAL BUG ALERT: Only checking time on layer 0
                //As long as everything is synce to that layer it should be okay though, right?
                if (!isSameState || GetNormalizedAnimTimeLeft(StateMachine, 0) < MinStateTime)
                {
                    //this means we've changed state since our action was
                    //played, we can issue our pending callback now.
                    CancelAnimationInternal();
                    return;
                }
            }


        }
        #endregion

        public enum PlayMode
        {
            Discard,
            Queue,
            Interrupt,
        }

        /// <summary>
        /// This assumes the Animator has already entered the desired state and now needs to scale a float param by the current animation
        /// in order to set it with a given execution time.
        /// </summary>
        /// <param name="executionTime"></param>
        public void SetAnimationScaledParam(float executionTime, int paramHash, int layer)
        {
            float animLen = GetCurrentAnimLength(StateMachine, layer);
            StateMachine.SetFloat(paramHash, (1 / executionTime) * SpeedScale * animLen);
        }

        /// <summary>
        /// Forces the backing animator to update.
        /// </summary>
        public void Pump()
        {
            StateMachine.Update(0.0f);
        }

        /// <summary>
        /// Plays an animation on this controller's Animator state machine. If
        /// another animation that started with this method is still executing, this
        /// will do nothing and return <c>false</c>. <see cref="CancelAnimation"/> can be
        /// used to prematurly stop an animation started with this method.
        /// </summary>
        /// <param name="nameHash">The name of the state to transition to. The transition will occur after the next frame Update.</param>
        /// <param name="param">The parameter of the internal Animator that is used to control the speed of the named animation state.</param>
        /// <param name="executionTime">A fixed period of time in which the animatio will play. Animation speed will be sped up or slowed down so that the entirety of the animation fits within this time.</param>
        /// <param name="fixedTime">If true, the execution time will represent an absolute time in which the animation plays, otherwise it is a speed scale factor of animation state.</param>
        /// <param name="callback">An optional callback that can be invoked once the animation has ended.</param>
        /// <returns><c>true</c> if the animation started successfully, <c>false</c> otherwise.</returns>
        public bool ExecuteAnimation(int nameHash, int layer, float executionTime, bool fixedTime, PlayMode mode = PlayMode.Discard, UnityAction callback = null)
        {
            if (!isActiveAndEnabled) return false;

            if (StateChangePending)
            {
                if (mode == PlayMode.Queue)
                {
                    QueueAnimation(nameHash, layer, executionTime, fixedTime, callback);
                    return true;
                }
                else if (mode == PlayMode.Discard)
                    return true;
                else if(mode == PlayMode.Interrupt)
                    CancelAnimationInternal();
            }

            if (executionTime <= 0) throw new UnityException("Providing a value of " + executionTime + " will cause the animation " + nameHash + " to never play or finish.");


            //play then pump the animator to ensure we move to the next state
            StateMachine.Play(nameHash, layer, 0.0f);
            StateMachine.Update(0.0f);

            //now that the state has changed we can set the playback parameter
            if (!fixedTime)
            {
                if(UseAnimatorSpeed)
                    StateMachine.speed = (1 / executionTime) * SpeedScale;
                else StateMachine.SetFloat(PlaybackSpeedParam.Hash, (1 / executionTime) * SpeedScale);
            }
            else
            {
                float animLen = GetCurrentAnimLength(StateMachine, layer);
                if (UseAnimatorSpeed)
                    StateMachine.speed = (1 / executionTime) * SpeedScale * animLen;
                else
                {
                    
                    StateMachine.SetFloat(PlaybackSpeedParam.Hash, (1 / executionTime) * SpeedScale * animLen);
                }

            }

            StateChangePending = true;
            Callback = callback;
            CurrentStatePending = nameHash;
            return true;
        }

        /// <summary>
        /// Queues an animation to play once the currently executing animation state has finished.
        /// Multiple calls can be made to this method in order to stack a series of animations that
        /// should play one after another.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="executionTime"></param>
        /// <param name="param"></param>
        /// <param name="fixedTime"></param>
        /// <param name="callback"></param>
        public void QueueAnimation(int nameHash, int layer, float executionTime, bool fixedTime, UnityAction callback = null)
        {
            AnimQueue.Enqueue(new AnimExecution(executionTime, fixedTime, layer, nameHash, callback));
        }

        /// <summary>
        /// Switches the Animator's state to the named one.
        /// </summary>
        /// <param name="name"></param>
        public void SwitchState(string name)
        {
            StateMachine.Play(name, 0, 1);
        }

        /// <summary>
        /// Switches the Animator's state to the named one.
        /// </summary>
        /// <param name="nameHash"></param>
        public void SwitchState(int nameHash)
        {
            StateMachine.Play(nameHash, 0, 1);
        }

        /// <summary>
        /// Returns true if the give name is the current state.
        /// (Internally checks the short state name)
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool IsCurrentState(int layerIndex, string stateName)
        {
            return StateMachine.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }

        /// <summary>
        /// Returns true if the give name is the current state.
        /// (Internally checks the short state name)
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool IsCurrentState(int layerIndex, int stateNameHash)
        {
            //we are right and truely fucked here. Unity has no way of checking for current state that is ambiguous of layer name.
            //Best we can do is check against the short hash name and never, ever, ever use sub-states. Fucking hell I fucking *hate* Mechanim.
            //Overengineered, wretched, fuck-stick, piece of garbage, fuck...
            //I mean, seriously?! What the fuck is the point of specifying the layer index if we have to *also* supply the layer in the full hash path?!
            //Mechanic can go get fucked.
            switch(NameHashType)
            {
                case NameHashTypes.Long:
                    {
                        return StateMachine.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateNameHash;
                    }
                case NameHashTypes.Short:
                    {
                        return StateMachine.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == stateNameHash;
                    }
                case NameHashTypes.Legacy:
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        return StateMachine.GetCurrentAnimatorStateInfo(layerIndex).nameHash == stateNameHash;
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                default:
                    {
                        return StateMachine.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == stateNameHash;
                    }
            }
                                       ;
        }

        /// <summary>
        /// Cancels the current animation started with <see cref="ExecuteAnimation"/>.
        /// This method must be called if you wish to play a new animation using
        /// PlayAnimation before a previously playing one has stopped. If there are any
        /// animations in the queue they will play next after this.
        /// </summary>
        public void CancelAnimation()
        {
            CancelAnimationInternal();
        }

        /// <summary>
        /// Removes all queued animations that are waiting to play.
        /// </summary>
        public void CancelQueue()
        {
            StateChangePending = false;
            Callback = null;
            AnimQueue.Clear();
        }

        /// <summary>
        /// Internal helper for reseting animator state.
        /// </summary>
        void CancelAnimationInternal()
        {
            CurrentStatePending = 0;
            StateChangePending = false;
            if (UseAnimatorSpeed)
                StateMachine.speed = 1.0f;
            else
                StateMachine.SetFloat(PlaybackSpeedParam, 1.0f);
            //Callback?.Invoke();
            if (Callback != null)
                Callback();

            if (AnimQueue.Count > 0)
            {
                var next = AnimQueue.Dequeue();
                ExecuteAnimation(next.StateNameHash, next.Layer, next.Time, next.FixedTime, PlayMode.Queue, next.Callback);
            }
        }


        #region Static Methods
        /// <summary>
        /// Returns the length of time in seconds of the current state's animation.
        /// </summary>
        /// <returns></returns>
        public static float GetCurrentAnimLength(Animator stateMachine, int layer)
        {
            var clips = stateMachine.GetCurrentAnimatorClipInfo(layer);
            return (clips.Length > 0) ? clips[0].clip.length : 0;
        }

        /// <summary>
        /// Returns the amount of time in seconds that has passed for the current state.
        /// </summary>
        /// <returns></returns>
        public static float GetCurrentAnimTimePassed(Animator stateMachine, int layer)
        {
            var animState = stateMachine.GetCurrentAnimatorStateInfo(layer);
            var clips = stateMachine.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                var clip = clips[0].clip;
                return clip.length * animState.normalizedTime;
            }
            return 0;
        }

        /// <summary>
        /// Returns the amount of time in seconds that is left of the current state.
        /// </summary>
        /// <returns></returns>
        public static float GetCurrentAnimTimeLeft(Animator stateMachine, int layer)
        {
            var animState = stateMachine.GetCurrentAnimatorStateInfo(layer);
            var clips = stateMachine.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                var clip = clips[0].clip;
                return clip.length * (1 - animState.normalizedTime);
            }
            return 0;
        }

        /// <summary>
        /// Returns the amount of time as a percentage that is left for the current state.
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <returns></returns>
        public static float GetNormalizedAnimTimeLeft(Animator stateMachine, int layer)
        {
            var animState = stateMachine.GetCurrentAnimatorStateInfo(layer);
            return (1.0f - animState.normalizedTime);
        }
        #endregion
    }
}
