using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public GameObject waypointPrefab;
    public List<Transform> waypoints = new List<Transform>();
    public bool isLooped = true;
    
    [Header("Visualization")]
    public Color pathColor = Color.yellow;
    public float lineWidth = 0.6f;
    
    private void OnDrawGizmos()
    {
        if (waypoints.Count < 2) return;
        
        Gizmos.color = pathColor;
        
        // Draw lines between waypoints
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i+1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
        }
        
        // If looped, connect the last waypoint to the first
        if (isLooped && waypoints.Count > 1 && waypoints[0] != null && waypoints[waypoints.Count-1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Count-1].position, waypoints[0].position);
        }
    }
    
    // Method to get all waypoints (used by AI)
    public Transform[] GetWaypoints()
    {
        return waypoints.ToArray();
    }
    public void RefreshWaypoints()
{
    waypoints.Clear();
    foreach (Transform child in transform)
    {
        waypoints.Add(child);
    }
}
}
