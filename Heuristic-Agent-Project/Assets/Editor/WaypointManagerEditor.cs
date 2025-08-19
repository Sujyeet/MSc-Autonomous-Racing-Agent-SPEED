using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor
{
    private WaypointManager waypointManager;
    private SerializedProperty waypointsProp;
    private SerializedProperty waypointPrefabProp;
    
    private void OnEnable()
    {
        waypointManager = (WaypointManager)target;
        waypointsProp = serializedObject.FindProperty("waypoints");
        waypointPrefabProp = serializedObject.FindProperty("waypointPrefab");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Waypoint System", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Draw default inspector properties
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Waypoint Tools", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Add buttons for waypoint management
        if (GUILayout.Button("Add Waypoint at End"))
        {
            AddWaypointAtEnd();
        }
        
        if (GUILayout.Button("Add Waypoint at Scene View"))
        {
            SceneView.duringSceneGui += PlaceWaypointInScene;
            EditorUtility.DisplayDialog("Add Waypoint", "Click in the Scene view to place a waypoint. Press Escape to cancel.", "OK");
        }
        
        if (GUILayout.Button("Snap All Waypoints to Ground"))
        {
            SnapAllWaypointsToGround();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Auto-Fix Tools", EditorStyles.boldLabel);
        
        // NEW: Auto-fix buttons
        if (GUILayout.Button("Auto-Collect All Child Waypoints"))
        {
            AutoCollectChildWaypoints();
        }
        
        if (GUILayout.Button("Rename All Waypoints Sequentially"))
        {
            RenameWaypointsSequentially();
        }
        
        if (GUILayout.Button("Sort Waypoints by Position"))
        {
            SortWaypointsByPosition();
        }
        
        if (GUILayout.Button("Remove Null/Missing Waypoints"))
        {
            RemoveNullWaypoints();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("One-Click Fix All", EditorStyles.boldLabel);
        
        // NEW: One-click fix all button
        if (GUILayout.Button("FIX ALL WAYPOINTS (Auto-Collect + Rename + Sort)", GUILayout.Height(30)))
        {
            FixAllWaypoints();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    // NEW: Auto-collect all child waypoints
    private void AutoCollectChildWaypoints()
    {
        Undo.RecordObject(waypointManager, "Auto-Collect Child Waypoints");
        
        // Get all child transforms that contain "waypoint" in their name (case insensitive)
        List<Transform> foundWaypoints = new List<Transform>();
        
        foreach (Transform child in waypointManager.transform)
        {
            if (child.name.ToLower().Contains("waypoint"))
            {
                foundWaypoints.Add(child);
            }
        }
        
        // Clear existing list and add found waypoints
        waypointManager.waypoints.Clear();
        waypointManager.waypoints.AddRange(foundWaypoints);
        
        EditorUtility.SetDirty(waypointManager);
        Debug.Log($"Auto-collected {foundWaypoints.Count} waypoints from children.");
    }
    
    // NEW: Sort waypoints by their position (useful for track loops)
    private void SortWaypointsByPosition()
    {
        if (waypointManager.waypoints.Count < 2) return;
        
        Undo.RecordObject(waypointManager, "Sort Waypoints by Position");
        
        // Sort waypoints by their distance from the first waypoint, following a path
        List<Transform> sortedWaypoints = new List<Transform>();
        List<Transform> remainingWaypoints = new List<Transform>(waypointManager.waypoints.Where(w => w != null));
        
        if (remainingWaypoints.Count == 0) return;
        
        // Start with the first waypoint
        Transform current = remainingWaypoints[0];
        sortedWaypoints.Add(current);
        remainingWaypoints.Remove(current);
        
        // Find the next closest waypoint each time
        while (remainingWaypoints.Count > 0)
        {
            Transform closest = null;
            float closestDistance = float.MaxValue;
            
            foreach (Transform waypoint in remainingWaypoints)
            {
                float distance = Vector3.Distance(current.position, waypoint.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = waypoint;
                }
            }
            
            if (closest != null)
            {
                sortedWaypoints.Add(closest);
                remainingWaypoints.Remove(closest);
                current = closest;
            }
            else
            {
                break;
            }
        }
        
        waypointManager.waypoints = sortedWaypoints;
        EditorUtility.SetDirty(waypointManager);
        Debug.Log($"Sorted {sortedWaypoints.Count} waypoints by position.");
    }
    
    // NEW: Remove null or missing waypoints
    private void RemoveNullWaypoints()
    {
        Undo.RecordObject(waypointManager, "Remove Null Waypoints");
        
        int originalCount = waypointManager.waypoints.Count;
        waypointManager.waypoints.RemoveAll(w => w == null);
        int removedCount = originalCount - waypointManager.waypoints.Count;
        
        EditorUtility.SetDirty(waypointManager);
        Debug.Log($"Removed {removedCount} null waypoints.");
    }
    
    // NEW: Fix all waypoints in one click
    private void FixAllWaypoints()
    {
        Debug.Log("=== FIXING ALL WAYPOINTS ===");
        
        // Step 1: Auto-collect child waypoints
        AutoCollectChildWaypoints();
        
        // Step 2: Remove null waypoints
        RemoveNullWaypoints();
        
        // Step 3: Sort by position
        SortWaypointsByPosition();
        
        // Step 4: Rename sequentially
        RenameWaypointsSequentially();
        
        Debug.Log("=== ALL WAYPOINTS FIXED ===");
        EditorUtility.DisplayDialog("Success", $"Fixed all waypoints! Found and organized {waypointManager.waypoints.Count} waypoints.", "OK");
    }
    
    private void PlaceWaypointInScene(SceneView sceneView)
    {
        Event e = Event.current;
        
        // Display help message
        Handles.BeginGUI();
        GUI.Label(new Rect(10, 10, 300, 40), "Click to place waypoints. Press Escape to exit placement mode.");
        Handles.EndGUI();
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // Cast a ray from the mouse position
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                // Create waypoint at hit position
                CreateWaypointAt(hit.point + Vector3.up * 0.1f);
                e.Use(); // Consume the event
            }
        }
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            SceneView.duringSceneGui -= PlaceWaypointInScene;
            e.Use();
        }
        
        sceneView.Repaint();
    }
    
    private void AddWaypointAtEnd()
    {
        Vector3 position;
        
        if (waypointManager.waypoints.Count > 0 && waypointManager.waypoints[waypointManager.waypoints.Count - 1] != null)
        {
            // Get the last waypoint's position
            Transform lastWaypoint = waypointManager.waypoints[waypointManager.waypoints.Count - 1];
            position = lastWaypoint.position + lastWaypoint.forward * 2f;
        }
        else
        {
            // If no waypoints, use the manager's position
            position = waypointManager.transform.position;
        }
        
        CreateWaypointAt(position);
    }
    
    private void CreateWaypointAt(Vector3 position)
    {
        // Check if prefab is assigned
        if (waypointManager.waypointPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a waypoint prefab first!", "OK");
            return;
        }
        
        // Create the waypoint
        GameObject waypointObj = PrefabUtility.InstantiatePrefab(waypointManager.waypointPrefab) as GameObject;
        waypointObj.transform.position = position;
        waypointObj.transform.parent = waypointManager.transform;
        waypointObj.name = "Waypoint_" + waypointManager.waypoints.Count;
        
        // Add to the list
        Undo.RecordObject(waypointManager, "Add Waypoint");
        waypointManager.waypoints.Add(waypointObj.transform);
        EditorUtility.SetDirty(waypointManager);
    }
    
    private void SnapAllWaypointsToGround()
    {
        Undo.RecordObject(waypointManager, "Snap Waypoints to Ground");
        
        foreach (Transform waypoint in waypointManager.waypoints)
        {
            if (waypoint == null) continue;
            
            // Cast a ray downward from the waypoint
            RaycastHit hit;
            if (Physics.Raycast(waypoint.position + Vector3.up * 10f, Vector3.down, out hit, 100f))
            {
                Undo.RecordObject(waypoint, "Snap to Ground");
                waypoint.position = hit.point + Vector3.up * 0.1f; // 0.1 units above the surface
                EditorUtility.SetDirty(waypoint);
            }
        }
        
        EditorUtility.SetDirty(waypointManager);
    }
    
    private void RenameWaypointsSequentially()
    {
        Undo.RecordObject(waypointManager, "Rename Waypoints Sequentially");
        
        for (int i = 0; i < waypointManager.waypoints.Count; i++)
        {
            if (waypointManager.waypoints[i] != null)
            {
                Undo.RecordObject(waypointManager.waypoints[i].gameObject, "Rename Waypoint");
                waypointManager.waypoints[i].gameObject.name = "Waypoint_" + i.ToString("00");
                EditorUtility.SetDirty(waypointManager.waypoints[i].gameObject);
            }
        }
        
        EditorUtility.SetDirty(waypointManager);
        Debug.Log($"Renamed {waypointManager.waypoints.Count} waypoints sequentially.");
    }
    
    // Draw lines between waypoints in the Scene view
    private void OnSceneGUI()
    {
        if (waypointManager.waypoints.Count < 2) return;
        
        Handles.color = waypointManager.pathColor;
        
        for (int i = 0; i < waypointManager.waypoints.Count; i++)
        {
            if (waypointManager.waypoints[i] == null) continue;
            
            // Draw position handle for each waypoint
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(waypointManager.waypoints[i].position, Quaternion.identity);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(waypointManager.waypoints[i], "Move Waypoint");
                waypointManager.waypoints[i].position = newPos;
                EditorUtility.SetDirty(waypointManager.waypoints[i]);
            }
            
            // Draw waypoint index
            Handles.Label(waypointManager.waypoints[i].position + Vector3.up * 0.5f, "Waypoint " + i);
        }
    }
}
