using UnityEngine;
using System.Collections;
using System;
using Sirenix.OdinInspector;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Flickers all renderers of this heirarchy.
    /// </summary>
    public class FlickerRenderers : AbstractOperationOnEvent
    {
        [Flags]
        public enum RendererTypes
        {
            Meshes      = 1,
            Sprites     = 1 << 1,
            Particles   = 1 << 2,
            Trails      = 1 << 3,
        }

        public float FlickerTime = 2;
        public float StartRate = 0.2f;
        public float EndRate = 0.01f;

        [SerializeField]
        [HideInInspector]
        private byte Renderers;

        [ShowInInspector]
        public bool Meshes
        {
            get { return (Renderers & (byte)RendererTypes.Meshes) != 0; }
            set
            {
                if (value) Renderers |= (byte)RendererTypes.Meshes;
                else Renderers &= ((byte)RendererTypes.Meshes ^ 0xff);
            }
        }

        [ShowInInspector]
        public bool Sprites
        {
            get { return (Renderers & (byte)RendererTypes.Sprites) != 0; }
            set
            {
                if (value) Renderers |= (byte)RendererTypes.Sprites;
                else Renderers &= ((byte)RendererTypes.Sprites ^ 0xff);
            }
        }

        [ShowInInspector]
        public bool Particles
        {
            get { return (Renderers & (byte)RendererTypes.Particles) != 0; }
            set
            {
                if (value) Renderers |= (byte)RendererTypes.Particles;
                else Renderers &= ((byte)RendererTypes.Particles ^ 0xff);
            }
        }

        [ShowInInspector]
        public bool Trails
        {
            get { return (Renderers & (byte)RendererTypes.Trails) != 0; }
            set
            {
                if (value) Renderers |= (byte)RendererTypes.Trails;
                else Renderers &= ((byte)RendererTypes.Trails ^ 0xff);
            }
        }

        public System.Action OnFlickerEnd;


        MeshRenderer[] MeshRends;
        SpriteRenderer[] SpriteRends;
#pragma warning disable CS0169 // The field 'FlickerRenderers.ParticleRends' is never used
        ParticleSystem[] ParticleRends;
        TrailRenderer[] TrailRends;
#pragma warning restore CS0169 // The field 'FlickerRenderers.ParticleRends' is never used

        protected override void Awake()
        {
            //TODO: Expose the arrays in the inspector and only populate these lists of they are empty at startup
            if(Meshes) MeshRends = GetComponentsInChildren<MeshRenderer>();
            if(Sprites) SpriteRends = GetComponentsInChildren<SpriteRenderer>();
            if (Trails) TrailRends = GetComponentsInChildren<TrailRenderer>();
            base.Awake();
        }

        private void OnEnable()
        {
            //WARNING: This assumes that all of these renderers were meant to be active by default!
            if (Meshes)
            {
                for (int i = 0; i < MeshRends.Length; i++)
                    MeshRends[i].enabled = true;
            }

            if (Sprites)
            {
                for (int i = 0; i < SpriteRends.Length; i++)
                    SpriteRends[i].enabled = true;
            }

            /*
            if (Particles)
            {
                for (int i = 0; i < ParticleRends.Length; i++)
                    ParticleRends[i].enabled = state;
            }
            */

            if (Trails)
            {
                for (int i = 0; i < TrailRends.Length; i++)
                    TrailRends[i].enabled = true;
            }
        }

        /*
        public void OnDisable()
        {
            StopAllCoroutines();
            if (OnFlickerEnd != null)
                OnFlickerEnd();
        }
        */

        /// <summary>
        /// Starts the flickering effect.
        /// </summary>
        public override void PerformOp()
        {
            if(FlickerTime < 0.0001f)
            {
                if (OnFlickerEnd != null)
                    OnFlickerEnd();
            }
            else StartCoroutine(Flicker());
        }

        IEnumerator Flicker()
        {
            float start = Time.time;
            float percent = 0;
            bool state = true;

            while (percent < 1)
            {
                percent = (Time.time - start) / FlickerTime;
                state = !state;

                if (MeshRends != null)
                {
                    for (int i = 0; i < MeshRends.Length; i++)
                        MeshRends[i].enabled = state;
                }

                if (SpriteRends != null)
                {
                    for (int i = 0; i < SpriteRends.Length; i++)
                        SpriteRends[i].enabled = state;
                }

                /*
                if (ParticleRends != null)
                {
                    for (int i = 0; i < ParticleRends.Length; i++)
                        ParticleRends[i].enabled = state;
                }
                */

                if(TrailRends != null)
                {
                    for (int i = 0; i < TrailRends.Length; i++)
                        TrailRends[i].enabled = state;
                }
                yield return new WaitForSeconds(Mathf.Lerp(StartRate, EndRate, percent));
            }

            if (OnFlickerEnd != null)
                OnFlickerEnd();
        }
    }
}
