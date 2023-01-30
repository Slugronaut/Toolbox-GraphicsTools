using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Toolbox.Graphics
{
    /// <summary>
    /// Converts a collection of sprites to a single mesh.
    /// All sprites must use the same texture or texture atlas.
    /// 
    /// This is an edit-time construct and will not perform any tasks
    /// during runtime. All code and data will be stripped from
    /// any builds and this will become an empty component. It is recommended
    /// that this component be removed after use.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    #if FULL_INSPECTOR
    public class SpritesToMesh : MonoBehaviour
    #else
    public class SpritesToMesh : MonoBehaviour
    #endif
    {
        #if UNITY_EDITOR
        public Color Tint = Color.white;
        public ColorSpace TintMode;

        public bool ApplySpriteRotations = true;


        public Sprite[] Sprites;

        public SpriteRenderer[] Rends;

        MeshFilter Filter;
        static string LastDir;

        void Reset()
        {
            Sprites = new Sprite[0];
        }

        void OnEnable()
        {
            if(!Application.isPlaying)
                Filter = GetComponent<MeshFilter>();
        }

        #if FULL_INSPECTOR
        [Button("Gather Sprites")]
        #endif
        void GatherChildSprites()
        {
            Rends = GetComponentsInChildren<SpriteRenderer>(true);
            Sprites = Rends.Select(x => x.sprite).ToArray();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        Mesh SpriteToMesh(Sprite sprite)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
            mesh.uv = sprite.uv;
            mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);
            return mesh;
        }

        /// <summary>
        /// 
        /// </summary>
        #if FULL_INSPECTOR
        [Button("Convert")]
        #endif
        void Convert()
        {
            SceneView s = SceneView.lastActiveSceneView;
            if(s == null)
            {
                Debug.Log("Cannot convert sprites when not in Scene view.");
                return;
            }
            bool old = s.in2DMode;
            s.in2DMode = true;

            if(Sprites == null || Sprites.Length < 0)
            {
                Debug.Log("There are no sprites to convert. Try gathering some first.");
                return;
            }

            List<Vector3> verts = new List<Vector3>(50);
            List<int> tris = new List<int>(50);
            List<Vector2> uvs = new List<Vector2>(50);

            List<Vector3> points = new List<Vector3>(20);
            List<int> inds = new List<int>(20);

            for (int i = 0; i < Sprites.Length; i++)
            {
                var sprite = Sprites[i];
                var rend = Rends[i];

                points.Clear();
                foreach (var p in sprite.vertices)
                {
                    Vector3 point;
                    if (ApplySpriteRotations)
                    {
                        point = p;
                        point.Scale(rend.transform.localScale);
                        point = rend.transform.rotation * point;
                        point += rend.transform.localPosition;
                    }
                    else point = rend.transform.position + (Vector3)p;
                    points.Add(point);
                }
                verts.AddRange(points);

                inds.Clear();
                foreach (var ind in sprite.triangles)
                    inds.Add(ind + (4 * i));
                tris.AddRange(inds);

                uvs.AddRange(sprite.uv);
            }
            

            //Debug.Log("Tris: " + tris.Count);
            Mesh mesh = new Mesh();
            string[] names = UnityEditor.FileUtil.GetUniqueTempPathInProject().Split('/');
            mesh.name = names[names.Length - 1];
            mesh.Clear();
            mesh.vertices = verts.ToArray();
            mesh.uv = uvs.ToArray();
            
            if(TintMode == ColorSpace.Uninitialized || TintMode == ColorSpace.Linear)
                mesh.colors = new Color[mesh.vertexCount].Select( x=> Tint.linear).ToArray();
            else mesh.colors = new Color[mesh.vertexCount].Select(x => Tint.gamma).ToArray();

            mesh.triangles = tris.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            s.in2DMode = old;

            if (string.IsNullOrEmpty(LastDir))
                LastDir = "Assets";
            string dir = EditorUtility.SaveFilePanelInProject("Save File", "Sprite2Mesh", "asset", "Choose location for converted mesh file.");
            if (!string.IsNullOrEmpty(dir))
            {
                names = dir.Split('/');
                mesh.name = names[names.Length - 1];
                LastDir = dir.Replace(mesh.name, "/" + string.Empty);
                mesh.name = mesh.name.Replace(".asset", string.Empty);
                AssetDatabase.CreateAsset(mesh, dir);
            }

            Filter.sharedMesh = mesh;
        }
        #endif
    }
}
