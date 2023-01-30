using UnityEngine;
using UnityEditor;
using System.Reflection;
using Toolbox.Graphics;

namespace Toolbox.ToolboxEditor
{
    /// <summary>
    /// Tool for easily painting sprites as decals.
    /// </summary>
    public class SpriteDecalEditor : EditorWindow
    {
        const int LEFT_MOUSE_BUTTON = 0;
        const int RIGHT_MOUSE_BUTTON = 1;
        const bool ColorFixer = false;
        const string CursorName = "Sprite Decal Cursor 10029";

        MethodInfo IntersectRayMesh = typeof(HandleUtility).GetMethod("IntersectRayMesh", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
        Sprite SpriteBrush;
        Material SpriteMat;

        //settings
        const float ColliderThickness = 0.0001f;
        const float MinOffset = 0;
        const float MaxOffset = 0.1f;
        float DecalOffset = 0.0015f;
        float OffsetJitter = 0.001f;
        bool ShareMaterial = true;
        ColorSpace ColorMode = ColorSpace.Gamma;

        float RotInc = 90;
        float RotOffset = 45;
        float MinRot = 0;
        float MaxRot = 360;
        float CurrRot = 45;

        float ScaleInc = 0.5f;
        float ScaleOffset = 0;
        float MinScale = 1;
        float MaxScale = 4;
        Vector2 CurrScale = new Vector2(3,4);


        GameObject Cursor;
        bool ParentDecals = false;
        Color CurrColor = Color.white;
        RotType Rot;
        ScaleType Scale;

        enum RotType
        {
            Fixed,
            Random,
        }

        enum ScaleType
        {
            Fixed,
            Random,
        }
        

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Window/Toolbox - Sprite Decals")]
        public static void MenuOpenDecalsWindow()
        {
            EditorWindow.GetWindow<SpriteDecalEditor>(false, "Sprite Decals", false).Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            //HookSceneView();
            SceneView.duringSceneGui += OnSceneGUI;
            
            //auto-load the first decal material we find
            var assets = AssetDatabase.FindAssets("Decal t:Material");
            if (assets != null && assets.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[0]);
                SpriteMat = AssetDatabase.LoadAssetAtPath<Material>(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            //SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.duringSceneGui -= this.OnSceneGUI;
            MurderCursor();
        }

        /// <summary>
        /// 
        /// </summary>
        void SetCursor(Sprite brush, Material mat, Color color)
        {
            SpriteRenderer sprite = null;
            if (Cursor == null)
            {
                Cursor = GameObject.Find(CursorName);
                if(Cursor == null) Cursor = new GameObject(CursorName);
                sprite = Cursor.AddComponent<SpriteRenderer>();
                Cursor.hideFlags = HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInInspector;
            }
            if(sprite == null) sprite = Cursor.GetComponent<SpriteRenderer>();
            sprite.sprite = brush;
            sprite.sharedMaterial = mat;
            if (ColorMode == ColorSpace.Uninitialized) sprite.color = color;
            else if (ColorMode == ColorSpace.Linear) sprite.color = color.linear;
            else if (ColorMode == ColorSpace.Gamma) sprite.color = color.gamma;
        }

        void MurderCursor()
        {
            if (Cursor != null) DestroyImmediate(Cursor);
            Cursor = null;
        }

        void OnGUI()
        {
            var rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(55));
            GUILayout.Space(1);
            EditorGUIUtility.DrawColorSwatch(rect, Cursor == null ? Color.red: Color.green);
            EditorGUILayout.EndHorizontal();
            if(EditorApplication.isPlayingOrWillChangePlaymode)
            {
                MurderCursor();
                EditorGUILayout.LabelField("\n\nCannot place decals in edit mode.\n\n");
                return;
            }
            
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            {
                //LEFT SIDE LAYOUT
                EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth/3));
                {
                    #region Rotation Settings
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Rotation", GUILayout.Width(60), GUILayout.ExpandWidth(false));
                        Rot = (RotType)EditorGUILayout.EnumPopup(Rot, GUILayout.Width(50));
                        if (Rot == RotType.Fixed)
                        {
                            //indent so that we have room to 'drag' the value
                            EditorGUI.indentLevel++;
                            CurrRot = EditorGUILayout.FloatField(CurrRot, GUILayout.Width(100));
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel += 5;
                            MinRot = EditorGUILayout.FloatField("Min", MinRot);
                            MaxRot = EditorGUILayout.FloatField("Max", MaxRot);
                            RotInc = EditorGUILayout.FloatField("Inc", RotInc);
                            RotOffset = EditorGUILayout.FloatField("Offset", RotOffset);
                            EditorGUI.indentLevel -= 5;
                            EditorGUILayout.BeginHorizontal();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion

                    GUILayout.Space(25);

                    #region Scale Settings
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Scale", GUILayout.Width(60), GUILayout.ExpandWidth(false));
                        Scale = (ScaleType)EditorGUILayout.EnumPopup(Scale, GUILayout.Width(50));
                        if (Scale == ScaleType.Fixed)
                        {
                            var scale = EditorGUILayout.Vector2Field("", CurrScale, GUILayout.Width(100));
                            CurrScale = new Vector3(scale.x, scale.y, 1);
                        }
                        else
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel += 5;
                            MinScale = EditorGUILayout.FloatField("Min", MinScale);
                            MaxScale = EditorGUILayout.FloatField("Max", MaxScale);
                            ScaleInc = EditorGUILayout.FloatField("Inc", ScaleInc);
                            ScaleOffset = EditorGUILayout.FloatField("Offset", ScaleOffset);
                            EditorGUI.indentLevel -= 5;
                            EditorGUILayout.BeginHorizontal();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion

                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Z-Offset", GUILayout.Width(60));
                    DecalOffset = EditorGUILayout.Slider(DecalOffset, MinOffset, MaxOffset);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Z-Jitter", GUILayout.Width(60));
                    OffsetJitter = EditorGUILayout.Slider(OffsetJitter, MinOffset, MaxOffset);
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(20);

                //RIGHT SIDE LAYOUT
                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.BeginChangeCheck();
                    SpriteBrush = EditorGUILayout.ObjectField("Decal", SpriteBrush, typeof(Sprite), true) as Sprite;
                    SpriteMat = EditorGUILayout.ObjectField("Material", SpriteMat, typeof(Material), false) as Material;
                    CurrColor = EditorGUILayout.ColorField("Tint", CurrColor);
                    if (EditorGUI.EndChangeCheck())
                        SetCursor(SpriteBrush, SpriteMat, CurrColor);
                    GUILayout.Space(10);
                    ColorMode = (ColorSpace)EditorGUILayout.EnumPopup("Color Space", ColorMode);
                    GUILayout.Space(10);
                    ParentDecals = EditorGUILayout.Toggle("Parent Decals", ParentDecals);
                    GUILayout.Space(10);
                    ShareMaterial = EditorGUILayout.Toggle("Share Material", ShareMaterial);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 
        /// </summary>
        void HookSceneView()
        {
            /*
            if (SceneView.onSceneGUIDelegate != this.OnSceneGUI)
            {
                SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
                SceneView.onSceneGUIDelegate += this.OnSceneGUI;
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="angle"></param>
        /// <param name="scale"></param>
        Transform CalculatePlacement(GameObject decal, Vector2 mousePos, float angle, Vector3 scale, string undoText)
        {
            GameObject nearest = HandleUtility.PickGameObject(mousePos, false, new GameObject[] { Cursor });
            if (nearest == null) return null;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            RaycastHit hit;
            MeshFilter mf = nearest.GetComponent<MeshFilter>();
            TerrainCollider tc = nearest.GetComponent<TerrainCollider>();
            SpriteRenderer sr = nearest.GetComponent<SpriteRenderer>();
            Vector3 finalPoint;

            if(undoText != null)
                Undo.RegisterCreatedObjectUndo(decal, undoText);

            //TODO: we should perform a sphere cast first to see if we overlap
            //any sprites - THEN perform mesh and terrain checks
            //I'm just not sure how to test for collision with a sprite
            //Use this -> HandleUtility.PickRectObjects()


            if (mf != null && mf.sharedMesh != null)
            {
                Mesh msh = mf.sharedMesh;
                
                // Use IntersectRayMesh because no other raycast is capable of intersecting non-collider objects.
                object[] parameters = new object[] { ray, msh, nearest.transform.localToWorldMatrix, null };
                if (IntersectRayMesh == null) IntersectRayMesh = typeof(HandleUtility).GetMethod("IntersectRayMesh", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
                object result = IntersectRayMesh.Invoke(this, parameters);

                if ((bool)result)
                {
                    hit = (RaycastHit)parameters[3];
                    finalPoint = hit.point;
                }

                else return null;
            }
            else if (sr != null)
            {
                //we're going to apply ad-hoc colliders to both the target and the source and use them in a boxcast
                Transform t = nearest.transform;
                Quaternion curRot = t.rotation;
                Vector3 curScale = t.localScale;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                var destBox = nearest.AddComponent<BoxCollider>();
                destBox.size = new Vector3(destBox.size.x, destBox.size.y, ColliderThickness);
                t.rotation = curRot;
                t.localScale = curScale;

                var decalBox = decal.AddComponent<BoxCollider>();
                decalBox.size = new Vector3(decalBox.size.x, decalBox.size.y, ColliderThickness);
                
                //if (!Physics.SphereCast(ray, Mathf.Max(decalBox.size.x, decalBox.size.y, decalBox.size.z), out hit))
                if(!Physics.Raycast(ray, out hit))
                {
                    DestroyImmediate(destBox);
                    DestroyImmediate(decalBox);
                    return null;
                }

                finalPoint = hit.point;

                //TODO:
                //we still need to check for the possibility of overlapping other sprites. We'll
                //need do a very shallow box check where our decal collider is to see if we need to
                //use another spite a a parent (take the 'highest' if there are several)

                DestroyImmediate(destBox);
                DestroyImmediate(decalBox);
            }
            else if (tc != null)
            {
                if (!Physics.Raycast(ray, out hit))
                    return null;
                else finalPoint = hit.point;
            }
            else return null;

            Vector3 point = finalPoint + (hit.normal.normalized * (DecalOffset + Random.Range(0, OffsetJitter)));
            PositionObject(decal.transform, point, scale, Quaternion.LookRotation(-hit.normal), angle);
            return nearest.transform;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="who"></param>
        /// <param name="pos"></param>
        /// <param name="scale"></param>
        /// <param name="rot"></param>
        /// <param name="angle"></param>
        void PositionObject(Transform who, Vector3 pos, Vector3 scale, Quaternion rot, float angle)
        {
            who.position = pos;
            who.localRotation = rot;
            Vector3 angles = who.eulerAngles;
            angles.z += angle;
            who.localRotation = Quaternion.Euler(angles);
            who.localScale = new Vector3(scale.x, scale.y, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        static GameObject CreateDecal(Sprite brush, Material mat, Color tint, ColorSpace mode, bool shareMat = true)
        {
            GameObject go = new GameObject("Decal " + brush.name);
            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = brush;
            if (shareMat) sprite.sharedMaterial = mat;
            else sprite.material = mat;
            if (mode == ColorSpace.Uninitialized) sprite.color = tint;
            else if (mode == ColorSpace.Linear) sprite.color = tint.linear;
            else if(mode == ColorSpace.Gamma) sprite.color = tint.gamma;
            return go;
        }

        void ApplyRandomizations()
        {
            if (Rot == RotType.Random)
            {
                CurrRot = Random.Range(Mathf.Min(MinRot, MaxRot), Mathf.Max(MinRot, MaxRot));

                //BUG ALERT: This is currently broken and will only work for 45/90
                //apply incrementation to rotation
                if (Mathf.Abs(RotInc) > 0 && Rot == RotType.Random)
                {
                    var r = (CurrRot % (RotInc / 2));
                    //Debug.Log("Raw R: " + r + "  Raw Rot: " + rot);
                    r =  CurrRot - r;
                    CurrRot = r * RotInc + RotOffset;
                    //Debug.Log("R: " + r + "  Rot: " + rot);
                }
            }
            if (Scale == ScaleType.Random)
            {
                //one random for all three dims for now
                CurrScale = Vector2.one * Random.Range(Mathf.Min(MinScale, MaxScale), Mathf.Max(MinScale, MaxScale));
            }

            
        }

        bool CursorOn = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneView"></param>
        void OnSceneGUI(SceneView sceneView)
        {
            // DO NOT USE THE EVENT
            // unitay needs shift clicks to register in order
            // to release the view mode shortcut
            Event e = Event.current;
            

            if (e.modifiers == EventModifiers.Shift)
            {
                if(!CursorOn)
                {
                    Repaint();
                    CursorOn = true;
                }
                if (Cursor == null) SetCursor(SpriteBrush, SpriteMat, CurrColor);
                if (e.type == EventType.MouseMove)
                {
                    //position the cursor in our prospective location
                    if (Cursor != null)
                        CalculatePlacement(Cursor, e.mousePosition, CurrRot, CurrScale, null);
                }
#if UNITY_STANDALONE_OSX
		EventModifiers em = e.modifiers;	// `&=` consumes the event.
		if( (em &= EventModifiers.Shift) != EventModifiers.Shift )
			return;

		int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlID);

		if( e.type == EventType.MouseUp && ((e.button == RIGHT_MOUSE_BUTTON && e.modifiers == EventModifiers.Shift) || (e.modifiers == (EventModifiers.Shift | EventModifiers.Control))) )
#else
                if (e.type == EventType.MouseUp && e.button == RIGHT_MOUSE_BUTTON)// && e.modifiers == EventModifiers.Shift)
#endif
                {
                    if (SpriteBrush != null)
                    {
                        GameObject decalGo = CreateDecal(SpriteBrush, SpriteMat, CurrColor, ColorMode, ShareMaterial);
                        var hit = CalculatePlacement(decalGo, e.mousePosition, CurrRot, CurrScale, "Undo Decal");
                        if (hit != null)
                        {
                            GameObjectUtility.SetStaticEditorFlags(decalGo, StaticEditorFlags.BatchingStatic | StaticEditorFlags.ContributeGI);
                            if (ParentDecals) decalGo.transform.SetParent(hit, true);
                            Selection.objects = new Object[1] { decalGo };
                            SceneView.RepaintAll();
                            ApplyRandomizations();
                        }
                        else DestroyImmediate(decalGo);
                    }
                }//end click-check
            }//end shift-check
            else
            {
                if(CursorOn)
                {
                    Repaint();
                    CursorOn = false;
                }
                MurderCursor();
            }
            
        }


    }
}

