using UnityEngine;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Simple script for making a flickering real-time light
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public class FlickeringLight : MonoBehaviour
    {
        public float IntensityVel = 0.1f;
        public float IntensityVarience = 0.0f;
        public float RangeVarience = 1.0f;
        public Color[] ColorDuty;
        public float Speed = 0.25f;
        Light l;

        int LastColor = 0;
        int NewColor = 0;
        float StartRange;
        float StartInten;
        float TargetInten;
        float LastTime;

        void OnEnable()
        {
            l = GetComponent<Light>();
            StartRange = l.range;
            StartInten = l.intensity;
            TargetInten = StartInten + Random.Range(-IntensityVarience, IntensityVarience);
        }
        
        void OnWillRenderObject()
        {
            float t = ((Time.time-LastTime)/Speed);
            l.intensity = Mathf.MoveTowards(l.intensity, TargetInten, IntensityVel);
            if(ColorDuty.Length > 0) l.color = Color.Lerp(ColorDuty[LastColor], ColorDuty[NewColor], t);
            
            if(Time.time - LastTime > Speed)
            {
                if (ColorDuty.Length > 0)
                {
                    LastColor = NewColor;
                    NewColor = Random.Range(0, ColorDuty.Length);
                }
                    
                l.range = Random.Range(StartRange - RangeVarience, StartRange + RangeVarience);
                TargetInten = StartInten + Random.Range(-IntensityVarience, IntensityVarience);
                LastTime = Time.time;
            }
        }
       
    }
}
