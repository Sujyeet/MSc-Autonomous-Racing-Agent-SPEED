using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointAutoGenerator))]
public class WaypointAutoGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaypointAutoGenerator generator = (WaypointAutoGenerator)target;
        
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Waypoint Generation", EditorStyles.boldLabel);
        
        if (generator.controlPoints.Count < 2)
        {
            EditorGUILayout.HelpBox("Add at least 2 control points along your track to generate waypoints.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox($"Preview shows {generator.numberOfWaypoints} waypoints. Adjust settings and click Generate.", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate Waypoints Along Track", GUILayout.Height(30)))
        {
            GenerateWaypoints(generator);
        }
        
        if (GUILayout.Button("Clear All Generated Waypoints"))
        {
            ClearWaypoints(generator);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Add Control Point at Scene View Center"))
        {
            AddControlPointAtSceneCenter(generator);
        }
    }
    
    void GenerateWaypoints(WaypointAutoGenerator generator)
    {
        if (generator.controlPoints.Count < 2)
        {
            EditorUtility.DisplayDialog("Error", "Please add at least 2 control points first!", "OK");
            return;
        }
        
        if (generator.waypointManager == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a WaypointManager!", "OK");
            return;
        }
        
        // Clear existing waypoints
        ClearWaypoints(generator);
        
        // Generate new waypoints
        for (int i = 0; i < generator.numberOfWaypoints; i++)
        {
            float t = (float)i / (generator.numberOfWaypoints - 1);
            Vector3 position = generator.GetSplinePoint(t);
            position.y += generator.waypointHeight;
            
            GameObject waypoint;
            if (generator.waypointPrefab != null)
            {
                waypoint = PrefabUtility.InstantiatePrefab(generator.waypointPrefab) as GameObject;
            }
            else
            {
                waypoint = new GameObject();
            }
            
            waypoint.name = $"Waypoint_{i:00}";
            waypoint.transform.position = position;
            waypoint.transform.parent = generator.waypointManager.transform;
            
            Undo.RegisterCreatedObjectUndo(waypoint, "Generate Waypoint");
        }
        
        // Refresh the waypoint manager
        generator.waypointManager.RefreshWaypoints();
        
        EditorUtility.SetDirty(generator.waypointManager);
        Debug.Log($"Generated {generator.numberOfWaypoints} waypoints along the track!");
    }
    
    void ClearWaypoints(WaypointAutoGenerator generator)
    {
        if (generator.waypointManager == null) return;
        
        // Remove all existing waypoints
        for (int i = generator.waypointManager.transform.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(generator.waypointManager.transform.GetChild(i).gameObject);
        }
        
        generator.waypointManager.waypoints.Clear();
        EditorUtility.SetDirty(generator.waypointManager);
    }
    
    void AddControlPointAtSceneCenter(WaypointAutoGenerator generator)
    {
        Vector3 sceneCenter = SceneView.lastActiveSceneView.camera.transform.position;
        
        GameObject controlPoint = new GameObject($"ControlPoint_{generator.controlPoints.Count}");
        controlPoint.transform.position = sceneCenter;
        controlPoint.transform.parent = generator.transform;
        
        generator.controlPoints.Add(controlPoint.transform);
        
        Undo.RegisterCreatedObjectUndo(controlPoint, "Add Control Point");
        EditorUtility.SetDirty(generator);
    }
}
