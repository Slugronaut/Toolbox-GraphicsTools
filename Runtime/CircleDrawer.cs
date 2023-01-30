using UnityEngine;


namespace Toolbox.Graphics
{
    /// <summary>
    /// Renders a circle centered on a GameObject.
    /// 
    /// TODO: Need to orient the cirle with regards to the transform.
    /// 
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    public class CircleDrawer : MonoBehaviour
    {
        public float Radius
        {
            get { return _Radius; }
            set
            {
                _Radius = value;
                if (_Radius < 0) _Radius = 0;
            }
        }
        [SerializeField]
        float _Radius = 1;

        public Color LineColor = Color.white;
        public float LineWidth = 0.02f;
        public Material Material;
        
        float ThetaScale = 0.01f;
        int Size;
        LineRenderer Renderer;
        

        void Awake()
        {
            //float sizeValue = (2.0f * Mathf.PI) / ThetaScale;
            //Size = (int)sizeValue;
            Size = (int)((1f / ThetaScale) + 2f);//1f);
            Size++;
            Renderer = gameObject.GetComponent<LineRenderer>();
            //if(Renderer.Renderer.material = new Material(Shader.Find("Particles/Additive"));
            Renderer.sharedMaterial = Material;
            Renderer.startWidth = 0.02f;
            Renderer.endWidth = 0.02f;
            Renderer.positionCount = Size;
        }

        void OnEnable()
        {
            Renderer.enabled = true;
        }

        void OnDisable()
        {
            Renderer.enabled = false;
        }

        void Update()
        {
            if (Material != Renderer.sharedMaterial) Renderer.sharedMaterial = Material;
            Vector3 pos;
            float theta = 0f;
            for(int i = 0; i < Size; i++)
            {
                theta += (2.0f * Mathf.PI * ThetaScale);
                float x = _Radius * Mathf.Cos(theta) * transform.localScale.x;
                float y = _Radius * Mathf.Sin(theta) * transform.localScale.y;
                x += gameObject.transform.position.x;
                y += gameObject.transform.position.y;
                pos = new Vector3(x, y, 0);
                Renderer.SetPosition(i, pos);
            }
            Renderer.startColor = LineColor;
            Renderer.endColor = LineColor;
            Renderer.startWidth = LineWidth;
            Renderer.endWidth = LineWidth;
        }

#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = LineColor;
            Gizmos.DrawWireSphere(Vector3.zero, _Radius);
        }

#endif
    }

}
