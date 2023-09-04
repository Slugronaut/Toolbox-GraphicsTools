using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Peg.Graphics
{
    /// <summary>
    /// Controls the alpha of the _Color property of all Renderers attached to this hierarchy.
    /// </summary>
    [ExecuteInEditMode]
    public class RendererAlphaProp : SerializedBehaviour, IRendererAlphaProp
    {
        [Tooltip("These renderers will not be alpha faded by this component.")]
        public Renderer[] Exclude;

        MeshRenderer[] Renderers;
        SpriteRenderer[] SpriteRends;
        //static Color MeshColor = Color.white;
        static MaterialPropertyBlock Block;
        static int PropId = Shader.PropertyToID("_Fade");

        /*
        [Tooltip("")]
        public bool UseNormal = true;
        [Tooltip("Used by the wall-fading system to determine which way this wall is facing. Walls that are perpendicular to the camera will not be faded.")]
        public Vector3 Normal;
        Transform Trans;

        /// <summary>
        /// The direction this object is 'facing' in world space.
        /// </summary>
        public Vector3 Facing
        {
            get
            {
                if (Trans == null) Trans = transform;
                return Trans.TransformVector(Normal);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(Toolbox.Math.MathUtils.GetObjectBounds(transform.position, gameObject, true, true, true).center, Facing*2);
        }
        */

        [SerializeField]
        float _Alpha = 1;
        [ShowInInspector]
        public float Alpha
        {
            get { return _Alpha; }
            set
            {
                if (isActiveAndEnabled && Renderers != null)
                {
                    if (_Alpha == value) return;

                    if (Block == null) Block = new MaterialPropertyBlock();

                    value = Mathf.Clamp(value, 0, 1);
                    _Alpha = value;
                    //MeshColor.a = value;
                    for (int i = 0; i < Renderers.Length; i++)
                    {
                        Renderers[i].GetPropertyBlock(Block);
                        //We need to know any previously set propertyblock color. Oddly enough,
                        //there is no 'GetColor'. Event more oddly, 'GetVector' works in place of it.
                        //var oldColor = Block.GetVector(PropId);
                        //Block.SetColor(PropId, new Color(oldColor.x, oldColor.y, oldColor.z, value).gamma);
                        Block.SetFloat(PropId, value);
                        Renderers[i].SetPropertyBlock(Block);
                    }

                    for (int i = 0; i < SpriteRends.Length; i++)
                    {
                        SpriteRends[i].GetPropertyBlock(Block);
                        Color c = SpriteRends[i].color;
                        SpriteRends[i].color = new Color(c.r, c.g, c.b, value);
                    }
                }
            }
        }
        

        void Awake()
        {
            GatherRenderers();
            Alpha = 1;
        }

        void Reset()
        {
            Exclude = new Renderer[0];
        }

        private void OnEnable()
        {
            if (Renderers == null || Renderers.Length < 1)
                GatherRenderers();
        }

        void OnDisable()
        {
            Alpha = 1;
        }

        void OnTransformChildrenChanged()
        {
            GatherRenderers();
        }

        /// <summary>
        /// Gathers all renderers that are children of this object excluding any that are
        /// on a GameObject marked with the DoNotAlphaFade component or are supplied in the
        /// exclusion list.
        /// </summary>
        void GatherRenderers()
        {
            //for some really odd reason, I can't just use 'Renderers', I have to separate meshes from sprites
            Renderers = GetComponentsInChildren<MeshRenderer>();
            if(Renderers != null)
                Renderers = Renderers.Where(x => x != null && (Exclude == null || !Exclude.Contains(x)) && x.gameObject.GetComponent<DoNotAlphaFade>() == null).ToArray();
            SpriteRends = GetComponentsInChildren<SpriteRenderer>();
            if(SpriteRends != null)
                SpriteRends = SpriteRends.Where(x => x != null && !Exclude.Contains(x) && x.gameObject.GetComponent<DoNotAlphaFade>() == null).ToArray();
            ForceUpdate();
        }

        /// <summary>
        /// Primarily used by the editor because Unity can be dumb somtimes.
        /// </summary>
        public void ForceUpdate()
        {
            Alpha = _Alpha;
        }
    }


    public interface IRendererAlphaProp
    {
        float Alpha { get; set; }
    }
}