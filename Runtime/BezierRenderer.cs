using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Peg.Graphics
{
    /// <summary>
    /// Renders a bezier curve that can be edited.
    /// </summary>
    [AddComponentMenu("Toolbox/Graphics/Bezier Renderer")]
    public class BezierRenderer : MonoBehaviour
    {
        public Transform StartPos;
        public Transform EndPos;
        public Vector3 Handle1;    // in relation to StartPos
        public Vector3 Handle2;    // in relation to EndPos
        public int Segments = 5;
        
        LineRenderer Rend;

        void Awake()
        {
            Rend = GetComponent<LineRenderer>();
            Rend.useWorldSpace = true;
            #if UNITY_5_5_OR_NEWER
            Rend.positionCount = Segments;
            #else
            Rend.SetVertexCount(Segments);
            #endif
        }

        void Update()
        {
            DrawCurve();
        }

        void DrawCurve()
        {
            Vector3 _from = StartPos.position;
            Vector3 piv_1 = StartPos.position - Handle1;
            Vector3 piv_2 = EndPos.position - Handle2;
            Vector3 _to = EndPos.position;
            Vector3 v;

            for (int i = 0; i < Segments; i++)
            {
                float point = 1.0f / Segments * i;
                v = CalculateBezierPoint(point, _from, piv_1, piv_2, _to);
                Rend.SetPosition(i, v);
            }

            v = CalculateBezierPoint(1, _from, piv_1, piv_2, _to);
            Rend.SetPosition(Segments - 1, v);

        }

        private void OnDrawGizmos()
        {
            Vector3 start = StartPos.position;
            Vector3 end = EndPos.position;
            Vector3 piv1 = StartPos.position - Handle1;
            Vector3 piv2 = EndPos.position - Handle2;
            Vector3 v = Vector3.zero;

            for (int i = 0; i < Segments; i++)
            {
                float point = 1.0f / Segments * i;
                v = CalculateBezierPoint(point, start, piv1, piv2, end);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(v, 0.1f);
                if (i > 0)
                {
                    point = 1.0f / Segments * (i - 1);
                    Vector3 prev = CalculateBezierPoint(point, start, piv1, piv2, end);
                    Gizmos.DrawLine(prev, v);
                }
            }

            Gizmos.DrawLine(v, end);
        }

        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }
    }
}