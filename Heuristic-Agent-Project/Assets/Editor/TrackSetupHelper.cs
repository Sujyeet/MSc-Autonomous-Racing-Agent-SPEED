#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TrackSetupHelper : EditorWindow
{
    [MenuItem("Tools/Track/Auto-Assign Layer & Collider")]
    public static void ShowWindow()
    {
        GetWindow<TrackSetupHelper>("Track Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Fix Track Colliders and Layers", EditorStyles.boldLabel);
        if (GUILayout.Button("Fix All Track Pieces"))
        {
            int fixedCount = 0;
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.StartsWith("ModularTrack"))
                {
                    Undo.RecordObject(obj, "Fix Track Object");

                    // Assign layer
                    obj.layer = LayerMask.NameToLayer("Track");

                    // Add MeshCollider if missing
                    if (obj.GetComponent<MeshCollider>() == null)
                    {
                        MeshFilter mf = obj.GetComponent<MeshFilter>();
                        if (mf && mf.sharedMesh)
                        {
                            MeshCollider mc = obj.AddComponent<MeshCollider>();
                            mc.sharedMesh = mf.sharedMesh;
                        }
                    }
                    fixedCount++;
                }
            }
            Debug.Log($"Fixed {fixedCount} track segments.");
        }
    }
}
#endif
