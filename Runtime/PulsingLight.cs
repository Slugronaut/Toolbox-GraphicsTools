using UnityEngine;


namespace Peg.Graphics
{
    /// <summary>
    /// Simple pulsing real-time light effect
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public class PulsingLight : MonoBehaviour
    {
        [Tooltip("X and Y are Min and Max respectively. Z is the velocity.")]
        public Vector3 IntensityRange;

        [Tooltip("X and Y are Min and Max respectively. Z is the velocity.")]
        public Vector3 DistanceRange;

        Light l;
        bool IntenUpswing;
        bool DistUpswing;

        void Start()
        {
            l = GetComponent<Light>();
        }

        void Update()
        {
            float t = Time.deltaTime;

            //intensity
            if (IntenUpswing)
            {
                l.intensity += IntensityRange.z * t;
                if (l.intensity > IntensityRange.y)
                {
                    l.intensity = IntensityRange.y;
                    IntenUpswing = false;
                }
            }
            else
            {
                l.intensity -= IntensityRange.z * t;
                if (l.intensity < IntensityRange.x)
                {
                    l.intensity = IntensityRange.x;
                    IntenUpswing = true;
                }
            }

            //range
            if (DistUpswing)
            {
                l.range += DistanceRange.z * t;
                if (l.range > DistanceRange.y)
                {
                    l.range = DistanceRange.y;
                    DistUpswing = false;
                }
            }
            else
            {
                l.range -= DistanceRange.z * t;
                if (l.range < DistanceRange.x)
                {
                    l.range = DistanceRange.x;
                    DistUpswing = true;
                }
            }
        }
    }
}
