using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Graphics
{
    /// <summary>
    /// Sets the color for all verticies of all instances the given mesh.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class MeshColorer : SerializedMonoBehaviour
    {
        #if UNITY_EDITOR

        [HideInInspector]
        [SerializeField]
        Color _Color;
        [ShowInInspector]
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    ColorMesh();
                }
            }
        }

        public int SubMeshIndex = -1;

        public MeshFilter Filter;

        void Reset()
        {
            SubMeshIndex = -1;
            if (Filter == null) Filter = GetComponent<MeshFilter>();
        }

        protected void Awake()
        {
            if(Filter == null) Filter = GetComponent<MeshFilter>();
        }

        void Start()
        {
            if (!Application.isPlaying && Filter != null) ColorMesh();
        }

        [Button("Change Color")]
        public void ColorMesh()
        {
            ColorVerts(SubMeshIndex);
        }

        public void ColorVerts(int index)
        {
            if (Filter == null) return;
            var mesh = Filter.sharedMesh;
            if (mesh == null) return;

            if (index < 0)
            {
                int count = mesh.vertexCount;
                Color[] colors = new Color[count];
                for (int i = 0; i < count; i++)
                    colors[i] = _Color;
                mesh.colors = colors;
            }
            else
            {
                var indicies = mesh.GetIndices(index);
                int count = indicies.Length;
                Color[] colors = new Color[count];
                for (int i = 0; i < count; i++)
                    colors[i] = _Color;

                var allColors = mesh.colors;
                int colorIndex = 0;
                foreach(var ind in indicies)
                {
                    allColors[ind] = colors[colorIndex];
                    colorIndex++;
                }

                mesh.colors = allColors;
            }
        }
        #endif
    }
}
