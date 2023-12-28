using Peg.MessageDispatcher;
using Peg.Messaging;
using UnityEngine;

namespace Peg.Graphics
{
    /// <summary>
    /// Listens for a command that instructs it to change layers weights on an animator.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorLayerWeights : LocalListenerMonoBehaviour
    {
        Animator Anim;


        void Awake()
        {
            DispatchRoot.AddLocalListener<ChangeAnimatorLayerWeightCmd>(HandleWeight);
            DispatchRoot.AddLocalListener<ChangeMultiAnimatorLayerWeightCmd>(HandleWeights);
            Anim = GetComponent<Animator>();
        }

        protected override void OnDestroy()
        {
            DispatchRoot.RemoveLocalListener<ChangeAnimatorLayerWeightCmd>(HandleWeight);
            DispatchRoot.RemoveLocalListener<ChangeMultiAnimatorLayerWeightCmd>(HandleWeights);
            base.OnDestroy();
        }

        void HandleWeight(ChangeAnimatorLayerWeightCmd cmd)
        {
            if(Anim.isInitialized)
                Anim.SetLayerWeight(cmd.Layer, cmd.Weight);
        }

        void HandleWeights(ChangeMultiAnimatorLayerWeightCmd cmd)
        {
            if (!Anim.isInitialized) return;
            int len = Mathf.Min(cmd.Layers.Length, cmd.Weights.Length);
            for (int i = 0; i < len; i++)
                Anim.SetLayerWeight(cmd.Layers[i], cmd.Weights[i]);
        }
    }


    /// <summary>
    /// Posted when a listening animator change change a layer weight.
    /// </summary>
    public class ChangeAnimatorLayerWeightCmd : IMessage
    {
        public static ChangeAnimatorLayerWeightCmd Shared = new ChangeAnimatorLayerWeightCmd(0, 0);
        public int Layer { get; private set; }
        public float Weight { get; private set; }

        public ChangeAnimatorLayerWeightCmd(int layer, float weight)
        {
            Layer = layer;
            Weight = weight;
        }

        public ChangeAnimatorLayerWeightCmd Change(int layer, float weight)
        {
            Layer = layer;
            Weight = weight;
            return this;
        }

    }


    /// <summary>
    /// Posted when a listening animator change change a layer weight.
    /// </summary>
    public class ChangeMultiAnimatorLayerWeightCmd : IMessage
    {
        public static ChangeMultiAnimatorLayerWeightCmd Shared = new ChangeMultiAnimatorLayerWeightCmd(null, null);
        public int[] Layers { get; private set; }
        public float[] Weights { get; private set; }

        public ChangeMultiAnimatorLayerWeightCmd(int[] layers, float[] weights)
        {
            Layers = layers;
            Weights = weights;
        }

        public ChangeMultiAnimatorLayerWeightCmd Change(int[] layers, float[] weights)
        {
            Layers = layers;
            Weights = weights;
            return this;
        }

    }
}
