using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Links shadow-casting only sprites to a main rendered sprite to keep animations in sync.
    /// </summary>
    [ExecuteInEditMode]
    public class SpriteShadowEffect : MonoBehaviour
    {
        [Serializable]
        public class Shadow
        {
            public SpriteRenderer ShadowRend;
            public bool MatchMirrorX;
            public ShadowCastingMode Mode;
        }
        public SpriteRenderer Rendered;
        public bool ReceiveShadows = true;
        public Shadow[] Shadows;

        // Use this for initialization
        void Start()
        {
            Rendered.shadowCastingMode = ShadowCastingMode.Off;
            Rendered.receiveShadows = ReceiveShadows;
            for (int i = 0; i < Shadows.Length; i++)
            {
                Shadows[i].ShadowRend.shadowCastingMode = Shadows[i].Mode;
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            for (int i = 0; i < Shadows.Length; i++)
            {
                Shadows[i].ShadowRend.sprite = Rendered.sprite;
                if (Shadows[i].MatchMirrorX) Shadows[i].ShadowRend.flipX = Rendered.flipX;
            }
        }
    }
}
