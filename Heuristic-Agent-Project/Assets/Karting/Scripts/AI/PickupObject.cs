using UnityEngine;

/// <summary>
/// Modified PickupObject to serve as a checkpoint for AI performance data collection
/// </summary>
public class PickupObject : TargetObject
{
    [Header("Checkpoint Data")]
    [Tooltip("Unique ID for this checkpoint")]
    public int checkpointID;
    
    [Tooltip("Name of the track segment (e.g., 'SpiralCurve', 'MainStraight')")]
    public string segmentName = "DefaultSegment";
    
    [Tooltip("Is this the start checkpoint of a segment?")]
    public bool isSegmentStart = false;
    
    [Tooltip("Is this the end checkpoint of a segment?")]
    public bool isSegmentEnd = false;
    
    [Header("Visual Feedback")]
    [Tooltip("VFX to spawn when checkpoint is triggered")]
    public GameObject spawnPrefabOnPickup;
    
    [Tooltip("Destroy the spawned VFX after this delay (seconds)")]
    public float destroySpawnPrefabDelay = 2f;
    
    // Track the last checkpoint time for segment timing
    private static float lastCheckpointTime = 0f;
    private static int lastCheckpointID = -1;

    void Start() {
        Register();
    }
    
    void OnCollisionEnter(Collision collision)
{
    // Check if it's the AI kart by layer only
    if ((layerMask.value & 1 << collision.gameObject.layer) > 0)
    {
        
        
        // Log checkpoint data directly
        Debug.Log($"Checkpoint {checkpointID} ({segmentName}) triggered by {collision.gameObject.name}");
    }
}
    void RecordCheckpointData(GameObject kart)
    {
        float currentTime = Time.time;
        float segmentTime = 0f;
        
        // Calculate time since last checkpoint
        if (lastCheckpointID >= 0)
        {
            segmentTime = currentTime - lastCheckpointTime;
        }
        
        // Update last checkpoint info
        lastCheckpointID = checkpointID;
        lastCheckpointTime = currentTime;
        
        // Find the AI agent's performance logger
        AI_Agent aiAgent = kart.GetComponent<AI_Agent>();
        if (aiAgent != null)
        {
            AIPerformanceLogger logger = kart.GetComponent<AIPerformanceLogger>();
            if (logger != null)
            {
                // Record checkpoint data in the existing logger
                logger.RecordCheckpoint(checkpointID, segmentName, isSegmentStart, isSegmentEnd, segmentTime);
                
                // Log for debugging
                Debug.Log($"Checkpoint {checkpointID} ({segmentName}) triggered by {kart.name}. Segment time: {segmentTime:F2}s");
            }
        }
    }
}
