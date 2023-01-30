using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

/*
public class BuildAnimatorOverrideWindow : UnityEditor.EditorWindow
{
    AnimatorOverrideController Controller;
    protected List<KeyValuePair<AnimationClip, AnimationClip>> overrides;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Build Animator Override")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BuildAnimatorOverrideWindow window = (BuildAnimatorOverrideWindow)EditorWindow.GetWindow(typeof(BuildAnimatorOverrideWindow));
        window.Show();
        window.titleContent = new GUIContent("Blahbeiddy");
    }
    string Filters;

    void OnGUI()
    {
        GUILayout.Space(12);

        Controller = EditorGUILayout.ObjectField("Override Controller", (UnityEngine.Object)Controller, typeof(AnimatorOverrideController), false) as AnimatorOverrideController;
        Filters = EditorGUILayout.TextField("Filters", Filters);
        GUILayout.Space(10);
        if (GUILayout.Button("Build", GUILayout.Width(200)))
        {
            overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(Controller.overridesCount);
            Controller.GetOverrides(overrides);
            for (int i = 0; i < overrides.Count; ++i)
            {
                //because of the way AnimationImporter works, we want underscores between names to improve our string matching
                string rootName = Controller.name.Replace(" ", "_");
                rootName = rootName + "_" + overrides[i].Key.name;
                string searchStr = Filters + " " + rootName;
                string filter = searchStr + " t:AnimationClip";
                //we want to search for an animation clip that matches our criteria
                //It  is a hueristic of our character name and the expected animation clip suffix.
                Debug.Log("Looking for: " + filter);
                var guids = AssetDatabase.FindAssets(filter);
                if (guids != null && guids.Length > 0)
                {
                    //because of the way the search filter works (numbers aren't taken into account for string matching)
                    //we need to narrow down any finds we have to the best match

                    //now thing get ugly, we have to find all paths to all found guids and
                    //see which one is the closest match to our original string
                    var paths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).Where(x => x != null).ToList();
                    string bestMatch = paths.OrderBy(s => string.Compare(s, searchStr)).First();

                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(bestMatch);
                    if (clip != null)
                    {
                        Debug.Log("Best Match for: <color=blue>" + searchStr + "</color> was found as <color=blue> " + bestMatch + "</color>");
                        overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, clip);
                    }
                    else Debug.Log("Best Match for: <color=yellow>" + searchStr + "</color> was not found as <color=yellow> " + bestMatch + "</color>");

                }
                else Debug.Log("No guids found");
            }
            Controller.ApplyOverrides(overrides);
        }

        GUILayout.Space(15);
    }
}
*/
