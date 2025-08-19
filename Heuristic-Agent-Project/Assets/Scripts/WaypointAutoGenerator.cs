using UnityEngine;
using System.Collections.Generic;

public class WaypointAutoGenerator : MonoBehaviour
{
    [Header("Control Points")]
    [Tooltip("Place a few control points along your track manually")]
    public List<Transform> controlPoints = new List<Transform>();
    
    [Header("Generation Settings")]
    [Range(10, 200)]
    public int numberOfWaypoints = 50;
    [Range(0.5f, 5f)]
    public float waypointHeight = 1.5f;
    [Range(0.1f, 2f)]
    public float smoothness = 0.5f;
    
    [Header("References")]
    public WaypointManager waypointManager;
    public GameObject waypointPrefab;
    
    [Header("Visualization")]
    public bool showPreview = true;
    public Color previewColor = Color.cyan;
    
    // Calculate position along spline using Catmull-Rom interpolation
    public Vector3 GetSplinePoint(float t)
    {
        if (controlPoints.Count < 2) return Vector3.zero;
        
        int segments = controlPoints.Count - 1;
        if (controlPoints[controlPoints.Count - 1].position == controlPoints[0].position)
            segments = controlPoints.Count;
        
        float scaledT = t * segments;
        int segment = Mathf.FloorToInt(scaledT);
        float localT = scaledT - segment;
        
        // Handle looping
        Vector3 p0 = GetControlPoint(segment - 1);
        Vector3 p1 = GetControlPoint(segment);
        Vector3 p2 = GetControlPoint(segment + 1);
        Vector3 p3 = GetControlPoint(segment + 2);
        
        return CatmullRom(p0, p1, p2, p3, localT);
    }
    
    Vector3 GetControlPoint(int index)
    {
        if (controlPoints.Count == 0) return Vector3.zero;
        
        // Handle looping for smooth spline
        if (index < 0)
            index = controlPoints.Count + index;
        if (index >= controlPoints.Count)
            index = index - controlPoints.Count;
            
        return controlPoints[index].position;
    }
    
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
    
    void OnDrawGizmos()
    {
        if (!showPreview || controlPoints.Count < 2) return;
        
        Gizmos.color = previewColor;
        
        // Draw preview of waypoint positions
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            float t = (float)i / (numberOfWaypoints - 1);
            Vector3 position = GetSplinePoint(t);
            position.y += waypointHeight;
            
            Gizmos.DrawWireSphere(position, 0.5f);
            
            if (i > 0)
            {
                float prevT = (float)(i - 1) / (numberOfWaypoints - 1);
                Vector3 prevPosition = GetSplinePoint(prevT);
                prevPosition.y += waypointHeight;
                Gizmos.DrawLine(prevPosition, position);
            }
        }
        
        // Draw control points
        Gizmos.color = Color.red;
        foreach (Transform cp in controlPoints)
        {
            if (cp != null)
                Gizmos.DrawSphere(cp.position, 0.8f);
        }
    }
}
