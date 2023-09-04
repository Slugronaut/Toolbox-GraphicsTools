using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Peg.Graphics
{
    /// <summary>
    /// Renders a polygon that represents a field-of-view cone. Optionally
    /// it can also adjust an attached polygon collider to match a simplfied
    /// version of the cone.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class FieldOfViewDrawer : MonoBehaviour
    {
        public enum Orientation
        {
            YUp,
            ZUp,
        }

        [SerializeField]
        float _Angle = 40;
        public float Angle
        {
            get { return _Angle * 2; }
            set
            {
                if (!Mathf.Approximately(value/2,_Angle))
                {
                    _Angle = Mathf.Min(value / 2, 180.0f);
                    if (_Angle < 0) _Angle = 0;
                    CacheOutdated = true;
                }
            }
        }

        [SerializeField]
        float _MinDist = 5.0f;
        public float MinDist
        {
            get { return _MinDist; }
            set
            {
                if (!Mathf.Approximately(value, _MinDist))
                {
                    _MinDist = Mathf.Max(value, 0.0f);
                    CacheOutdated = true;
                }
            }
        }

        [SerializeField]
        float _MaxDist = 15.0f;
        public float MaxDist
        {
            get { return _MaxDist; }
            set
            {
                if (!Mathf.Approximately(value, _MaxDist))
                {
                    _MaxDist = Mathf.Max(value, _MinDist);
                    CacheOutdated = true;
                }
            }
        }

        [Tooltip("Determines which axis the mesh considers to be up for the sake of rendering. Usually 2D will want 'ZUp' and 3D will want 'YUp'.")]
        [SerializeField]
        Orientation _Alignment;
        public Orientation Alignment
        {
            get { return _Alignment; }
            set
            {
                if (value != _Alignment)
                {
                    _Alignment = value;
                    CacheOutdated = true;
                }
            }
        }

        [SerializeField]
        Material _material;
        public Material material
        {
            get { return _material; }
            set
            {
                if(value != _material)
                {
                    _material = value;
                    CacheOutdated = true;
                }
            }
        }
        

        public int Layer;
        [Tooltip("If set, an attached polygon collider will be set roughly match the arc drawn.")]
        public bool MatchPolygonCollider = true;


        bool CacheOutdated = true;
        int Quality
        {
            get { return Mathf.CeilToInt(_Angle / 2); } 
        }
        int OldQuality;
        Mesh Mesh;
        MeshFilter Filter;

        void InitMesh(bool newMesh)
        {
            //OLD if(newMesh) mesh = new Mesh();
            if(newMesh)
            {
                Mesh = Filter.sharedMesh;
                if (Mesh == null) Mesh = new Mesh();
            }
            Mesh.Clear();
            Mesh.vertices = new Vector3[4 * Quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
            Mesh.triangles = new int[3 * 2 * Quality];

            Vector3[] normals = new Vector3[4 * Quality];
            Vector2[] uv = new Vector2[4 * Quality];

            for (int i = 0; i < uv.Length; i++)
                uv[i] = new Vector2(0, 0);
            for (int i = 0; i < normals.Length; i++)
                normals[i] = new Vector3(0, 1, 0);

            Mesh.uv = uv;
            Mesh.normals = normals;
            OldQuality = Quality;
            CacheOutdated = true;
        }

        void OnEnable()
        {
            Filter = GetComponent<MeshFilter>();
            InitMesh(true);
        }

        void Update()
        {
            if (Mesh == null) InitMesh(true);
            else if (OldQuality != Quality) InitMesh(false);


            if (CacheOutdated)
            {
                GetComponent<MeshRenderer>().sharedMaterial = material;
                CacheOutdated = false;
                float angle_lookat = GetAngle();

                float angle_start = angle_lookat - _Angle;
                float angle_end = angle_lookat + _Angle;
                float angle_delta = (angle_end - angle_start) / Quality;

                float angle_curr = angle_start;
                float angle_next = angle_start + angle_delta;

                Vector3 pos_curr_min = Vector3.zero;
                Vector3 pos_curr_max = Vector3.zero;

                Vector3 pos_next_min = Vector3.zero;
                Vector3 pos_next_max = Vector3.zero;

                Vector3[] vertices = new Vector3[4 * Quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
                int[] triangles = new int[3 * 2 * Quality];

                //polygon collider setup
                PolygonCollider2D col = null;
                List<Vector2> points = null;
                if (this.MatchPolygonCollider)
                {
                    col = GetComponent<PolygonCollider2D>();
                    points = new List<Vector2>(25);
                    points.Add(Vector2.zero);
                }

                for (int i = 0; i < Quality; i++)
                {
                    Vector3 sphere_curr = new Vector3
                        (
                        Mathf.Sin(Mathf.Deg2Rad * (angle_curr)),
                        Alignment == Orientation.YUp ? 0 : Mathf.Cos(Mathf.Deg2Rad * (angle_curr)),  // Left handed CW
                        Alignment == Orientation.ZUp ? 0 : Mathf.Cos(Mathf.Deg2Rad * (angle_curr))
                        );

                    Vector3 sphere_next = new Vector3
                        (
                        Mathf.Sin(Mathf.Deg2Rad * (angle_next)),
                        Alignment == Orientation.YUp ? 0 : Mathf.Cos(Mathf.Deg2Rad * (angle_next)),
                        Alignment == Orientation.ZUp ? 0 : Mathf.Cos(Mathf.Deg2Rad * (angle_next))
                        );

                    pos_curr_min = sphere_curr * _MinDist;
                    pos_curr_max = sphere_curr * _MaxDist;

                    pos_next_min = sphere_next * _MinDist;
                    pos_next_max = sphere_next * _MaxDist;

                    int a = 4 * i;
                    int b = 4 * i + 1;
                    int c = 4 * i + 2;
                    int d = 4 * i + 3;

                    vertices[a] = pos_curr_min;
                    vertices[b] = pos_curr_max;
                    vertices[c] = pos_next_max;
                    vertices[d] = pos_next_min;

                    triangles[6 * i] = a;       // Triangle1: abc
                    triangles[6 * i + 1] = b;
                    triangles[6 * i + 2] = c;
                    triangles[6 * i + 3] = c;   // Triangle2: cda
                    triangles[6 * i + 4] = d;
                    triangles[6 * i + 5] = a;


                    //calculate outer bounds of the collider
                    //if (i == 0 && col != null) p[1] = pos_curr_max;
                    //else if (i == Quality - 1 && col != null) p[2] = pos_next_max;
                    if (MatchPolygonCollider && col != null)
                    {
                        if (i == 0) points.Add(pos_curr_max);
                        else if (i == Quality - 1) points.Add(pos_next_max);
                        else
                        {
                            //only add enough extra points to get a good rough shape.
                            if(i == Quality / 2) points.Add(pos_curr_max);
                        }
                    }

                    angle_curr += angle_delta;
                    angle_next += angle_delta;

                }

                if(col != null) col.SetPath(0, points.ToArray());
                Mesh.Clear();
                ScaleVerts(vertices, transform.lossyScale);
                Mesh.vertices = vertices;
                Mesh.triangles = triangles;
                Filter.sharedMesh = Mesh;

            }
            //Graphics.DrawMesh(mesh, transform.position, transform.rotation, material, Layer);
        }

        float GetAngle()
        {
            return 90 - Mathf.Rad2Deg * Mathf.Atan2(transform.forward.z, transform.forward.x); // Left handed CW. z = angle 0, x = angle 90
        }

        static void ScaleVerts(Vector3[] verts, Vector3 scale)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i].Scale(scale);
            }
        }
    }
}