#if CRIMSON
using System;
using System.Collections.Generic;
using Toolbox.Common;
using UnityEngine;
using Crimson;

namespace Toolbox.Extensions.Crimson
{
    /// <summary>
    /// Spawns a set of sprites at each vertex of a supplied mesh.
    /// Spawning is an edit-time only feature. During runtime, this
    /// script will immediately destroy itself upon starting.
    /// 
    /// WARNING: This script assumes all child objects are generated
    /// by itself and will destroy them when enabled in the editor.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class SpriteCoveredMesh : MonoBehaviour
    {
#if UNITY_EDITOR
        [Serializable]
        public class Pass
        {
            public enum BillboardType
            {
                None,
                CameraCulled,
                SingleTarget,
            }
            public Sprite Prefab;
            public Material Mat;
            public Color Tint;
            public Vector3 Scale = Vector3.one;
            public BillboardType Billboard = BillboardType.CameraCulled;
            public Transform FaceTarget;
            public bool RuntimeBillboard = false;
            public bool LocalRotation = false;
            public uint FrameSkip = 4;
            [Compact]
            public Vector3 Offset;
        }
        

        [Tooltip("Each pass represents a set of sprites attached to each of the mesh's verticies.")]
        public Pass[] Passes;
        
        [HideInInspector]
        public bool Initialized;

        [Tooltip("Any child objects in this list will be left untouched when generating new sprites.")]
        public List<Transform> IgnoreChildren;


        private void Reset()
        {
            IgnoreChildren = new List<Transform>();
        }

        void OnAwake()
        {
            Init();
        }

        public void Cleanup()
        {
            if (Application.isPlaying || Initialized) return;
            
            Transform child;
            int inc = 0;
            while (transform.childCount > IgnoreChildren.Count)
            {
                child = transform.GetChild(inc);
                if (IgnoreChildren.Contains(child)) inc++;
                else DestroyImmediate(child.gameObject);
                
            }
        }

        public void Init()
        {
            //we only apply this effect while in edit-mode
            if (Application.isPlaying || Initialized) return;
            Cleanup();
            if (Passes == null || Passes.Length < 1) return;

            //Debug.Log("Generating " + Passes.Length + " passes of sprites for mesh on " + name + ".");
            Vector3 origin = transform.position;
            var filter = GetComponentInChildren<MeshFilter>();
            var verts = filter.sharedMesh.vertices;
            int passCount = 0;
            foreach (var pass in Passes)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    var go = new GameObject(pass.Prefab.name + "[" + passCount + ","+ i + "]");
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = verts[i] + pass.Offset;
                    var sprite = go.AddComponent<SpriteRenderer>();
                    sprite.color = pass.Tint;
                    sprite.sharedMaterial = pass.Mat;
                    sprite.sprite = pass.Prefab;
                    sprite.transform.localScale = pass.Scale;

                    if (pass.Billboard == Pass.BillboardType.CameraCulled)
                    {
                        var bill = go.AddComponent<Billboard>();
                        bill.UpdateOnlyOnEnable = !pass.RuntimeBillboard;
                        bill.LocalRot = pass.LocalRotation;
                        bill.FrameSkip = (int)pass.FrameSkip;
                    }
                    else if (pass.Billboard == Pass.BillboardType.SingleTarget)
                    {
                        var bill = go.AddComponent<BillboardNoRender>();
                        bill.FaceTarget = pass.FaceTarget;
                        bill.UpdateOnlyOnEnable = !pass.RuntimeBillboard;
                        bill.LocalRot = pass.LocalRotation;
                        bill.FrameSkip = (int)pass.FrameSkip;
                    }
                }
                passCount++;
            }
            Initialized = true;
        }
#endif
    }

}
#endif