using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Controls alpha settings of all child sprite renderers.
    /// </summary>
    [ExecuteInEditMode]
    public class SpriteCollectionParams : SerializedMonoBehaviour
    {
        [SerializeField]
        float _Alpha;

        [ShowInInspector]
        public float Alpha
        {
            get { return _Alpha; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                _Alpha = value;
                for (int i = 0; i < Sprites.Count; i++)
                {
                    Color c = Sprites[i].color;
                    Sprites[i].color = new Color(c.r, c.g, c.b, value);
                }
            }
        }

        //List<SpriteRenderer> Sprites;
        List<SpriteRenderer> Sprites;

        void Start()
        {
            Sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        }

        private void OnTransformChildrenChanged()
        {
            Sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        }

        /// <summary>
        /// Primarily used by the editor because Unity can be dumb somtimes.
        /// </summary>
        public void ForceUpdate()
        {
            Alpha = _Alpha;
        }
    }
}