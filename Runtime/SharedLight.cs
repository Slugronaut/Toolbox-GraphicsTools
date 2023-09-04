using UnityEngine;
using Sirenix.OdinInspector;

namespace Peg.Graphics
{
    /// <summary>
    /// Allows setting of light parameters on a group of lights using only a single component.
    /// </summary>
    public class SharedLight : SerializedMonoBehaviour
    {
        
        [SerializeField]
        Color _Color;
        [ShowInInspector]
        public Color Color
        {
            get { return _Color; }
            set
            {
                for (int i = 0; i < Lights.Length; i++)
                    Lights[i].color = value;
                _Color = value;
            }
        }

        [SerializeField]
        float _Range;
        [ShowInInspector]
        public float Range
        {
            get { return _Range; }
            set
            {
                for (int i = 0; i < Lights.Length; i++)
                    Lights[i].range = value;
                _Range = value;
            }
        }

        [SerializeField]
        float _Intensity;
        [ShowInInspector]
        public float Intensity
        {
            get { return _Intensity; }
            set
            {
                for (int i = 0; i < Lights.Length; i++)
                    Lights[i].intensity = value;
                _Intensity = value;
            }
        }

        [SerializeField]
        LightType _Type;
        [ShowInInspector]
        public LightType Type
        {
            get { return _Type; }
            set
            {
                for (int i = 0; i < Lights.Length; i++)
                    Lights[i].type = value;
                _Type = value;
            }
        }

        public Light[] Lights;

        void Reset()
        {
            Lights = gameObject.GetComponentsInChildren<Light>();
            if (Lights == null) Lights = new Light[0];
            Color = Color.black;
            Range = 10;
            Intensity = 1;
            Type = LightType.Point;
        }
    }
}
