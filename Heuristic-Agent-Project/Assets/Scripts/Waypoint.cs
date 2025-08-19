using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public Color waypointColor = Color.yellow;
    public float waypointSize = 0.3f;
    
    // Optional: Add properties for AI behavior at this waypoint
    public float speedModifier = 1f;
    
    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize the waypoint
        Gizmos.color = waypointColor;
        Gizmos.DrawSphere(transform.position, waypointSize);
    }
}
