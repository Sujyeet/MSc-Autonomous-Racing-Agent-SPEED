using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoWaypointPlacer : MonoBehaviour
{
    public WaypointManager waypointManager;
    public GameObject waypointPrefab;
    public Transform startPoint;
    public float spacing = 1.5f;
    public int waypointCount = 50;

    // Draw a button in the Inspector
    [ContextMenu("Place Waypoints Along White Line")]
    public void PlaceWaypoints()
    {
        if (waypointManager == null || startPoint == null)
        {
            Debug.LogError("Assign WaypointManager and StartPoint!");
            return;
        }

        // Example: Place waypoints in a straight line (replace with your logic)
        Vector3 direction = startPoint.forward;
        Vector3 position = startPoint.position;

        // Clear old waypoints
        for (int i = waypointManager.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(waypointManager.transform.GetChild(i).gameObject);
        }
        waypointManager.waypoints.Clear();

        for (int i = 0; i < waypointCount; i++)
        {
            GameObject wp = Instantiate(waypointPrefab, position, Quaternion.identity, waypointManager.transform);
            wp.name = $"Waypoint_{i:00}";
            waypointManager.waypoints.Add(wp.transform);
            position += direction * spacing;
        }
        Debug.Log($"Placed {waypointCount} waypoints.");
    }
}
