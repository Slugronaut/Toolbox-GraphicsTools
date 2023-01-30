using UnityEngine;
using Toolbox;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Allows a screen-space UI element to follow a world-space target onscreen.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class WorldUIElement : MonoBehaviour
    {
        [Tooltip("The worlspace target that this screen-space")]
        public Transform Target;

        [Tooltip("A screen-space offset to apply after moving this element.")]
        public Vector2 Offset;

        [Tooltip("A smoothing factor to apply to the UI element when adjusting its position. Can be used to avoid sudden, jerky motions due to rounding error when converting from world-space to screen-space.")]
        public float Smoothing = 0;

        RectTransform Rect;
        Vector2 Last;


        void Awake()
        {
            Rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (Target != null)
            {
                Vector2 p1 = Camera.main.WorldToScreenPoint(Target.position);
                p1 += Offset;

                if (Smoothing > 0)
                {
                    float delta = Time.deltaTime;
                    Vector2 curr = Rect.position;
                    float x = Math.MathUtils.SmoothApproach(curr.x, Last.x, p1.x, Smoothing, delta);
                    float y = Math.MathUtils.SmoothApproach(curr.y, Last.y, p1.y, Smoothing, delta);
                    curr = new Vector2(x, y);
                    Last = curr;
                    Rect.position = curr;
                }
                else
                {
                    Rect.position = p1;
                    Last = Rect.position;
                }
            }
        }
    }
}
