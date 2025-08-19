using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI_Agent : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private Transform waypointsParent;
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float waypointThreshold = 2f;
    [SerializeField] private float lookAheadDistance = 5f;

    [Header("Physics Settings")]
    [SerializeField] private float accelerationForce = 50f;
    [SerializeField] private float brakingForce = 300f;
    [SerializeField] private float steeringResponse = 2.5f;
    [SerializeField] private float lateralFriction = 5f;
    
    [Header("Simulation Settings")]
    [SerializeField] private float simulationSpeed = 1.0f;
    [SerializeField] private bool autoRestartOnCompletion = false;
    [SerializeField] private int numberOfTrials = 5;
    [SerializeField] private float delayBetweenTrials = 2f;
    
    [Header("Loop Settings")]
    [SerializeField] private int targetLaps = 3;
    [SerializeField] private bool enableLapTiming = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showWaypointGizmos = true;

    private Rigidbody rb;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private bool reachedEnd = false;
    private AIPerformanceLogger performanceLogger;
    private float currentSpeed;
    [HideInInspector] private float targetSteerAngle = 0f;
    private Vector3 lastPosition;
    private float totalTravelDistance = 0f;
    private float idealPathLength = 0f;
    private float pathDeviation = 0f;
    private int frameCounter = 0;
    private float runningTime = 0f;
    private float averageSteeringAngle = 0f;
    private int steeringAngleSamples = 0;
    private float maxSteeringAngle = 0f;
    private float timeSpentBraking = 0f;
    private int currentTrialNumber = 1;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float originalFixedDeltaTime;
    private WheelCollider[] wheelColliders;
    private float initialGroundY;
    private bool startupPhase = false; 
    private float startupTimer = 0f;   

    // Loop variables
    private int currentLap = 0;
    private int completedLaps = 0;
    private float lapStartTime = 0f;
    private List<float> lapTimes = new List<float>();
    private bool hasStartedFirstLap = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        performanceLogger = GetComponent<AIPerformanceLogger>();
        lastPosition = transform.position;
        startPosition = transform.position;
        startRotation = transform.rotation;
        originalFixedDeltaTime = Time.fixedDeltaTime;
        wheelColliders = GetComponentsInChildren<WheelCollider>();
        initialGroundY = transform.position.y - 0.5f;
        
        // Configure Rigidbody for stable movement
        if (rb != null)
        {
            rb.mass = 1000f;
            rb.centerOfMass = new Vector3(0, -0.6f, -0.2f);
            rb.drag = 0.2f;
            rb.angularDrag = 1.0f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        ConfigureWheelColliders();
        
        currentSpeed = baseSpeed * 0.3f;
        InitializeWaypoints();
        CalculateIdealPathLength();
        SetSimulationSpeed(simulationSpeed);
        
        if (performanceLogger != null)
        {
            performanceLogger.LogExportPath();
        }
        
        Debug.Log($"Starting Trial {currentTrialNumber} of {numberOfTrials}");
        StartCoroutine(SmoothStart());
    }
    
    void ConfigureWheelColliders()
    {
        if (wheelColliders == null || wheelColliders.Length == 0)
            return;
            
        foreach (WheelCollider wheel in wheelColliders)
        {
            JointSpring spring = wheel.suspensionSpring;
            spring.spring = spring.spring * 1.5f;
            spring.damper = spring.damper * 1.2f;
            wheel.suspensionSpring = spring;
            
            wheel.suspensionDistance = 0.2f;
            
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.stiffness = 2.0f;
            wheel.sidewaysFriction = sidewaysFriction;
            
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.stiffness = 1.5f;
            wheel.forwardFriction = forwardFriction;
        }
    }
    
    IEnumerator SmoothStart()
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = false;
        
        rb.AddForce(Vector3.down * rb.mass * 9.81f * 3f, ForceMode.Impulse);
        
        float startupDuration = 2.0f;
        startupPhase = false;
        startupTimer = 0f;
        
        while (startupTimer < startupDuration)
        {
            startupTimer += Time.deltaTime;
            float t = startupTimer / startupDuration;
            currentSpeed = Mathf.Lerp(baseSpeed * 0.1f, baseSpeed, t);
            
            if (!rb.isKinematic)
            {
                Vector3 forcePos = transform.position;
                forcePos.y -= 0.5f;
                rb.AddForceAtPosition(transform.forward * accelerationForce * 0.2f, forcePos, ForceMode.Acceleration);
                rb.AddForce(Vector3.down * rb.mass * 9.81f * 1.5f, ForceMode.Force);
            }
            
            yield return null;
        }
        
        startupPhase = false;
        currentSpeed = baseSpeed;
    }

    public float GetSteerInput()
    {
        return Mathf.Clamp(targetSteerAngle / 45f, -1f, 1f);
    }

    public void SetSimulationSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, 0.1f, 2.5f);
        Time.timeScale = speed;

        if (speed > 1.5f)
        {
            Time.fixedDeltaTime = 0.01f * Mathf.Sqrt(speed);
        }
        else
        {
            Time.fixedDeltaTime = 0.01f * speed;
        }

        if (speed > 1.5f)
        {
            Physics.defaultSolverIterations = 10;
            Physics.defaultSolverVelocityIterations = 3;
        }
        else
        {
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;
        }

        simulationSpeed = speed;
        Debug.Log($"Simulation speed set to {speed}x (fixedDeltaTime: {Time.fixedDeltaTime})");
    }

    void InitializeWaypoints()
    {
        if (waypointsParent != null)
        {
            foreach (Transform child in waypointsParent)
                if (child != waypointsParent)
                    waypoints.Add(child);

            waypoints = waypoints.OrderBy(w => {
                string numString = new string(w.name.Where(char.IsDigit).ToArray());
                if (int.TryParse(numString, out int number))
                    return number;
                return 0;
            }).ToList();
            
            if (waypoints.Count > 0)
                Debug.Log($"Initialized {waypoints.Count} waypoints");
            else
                Debug.LogWarning("No waypoints found! Make sure waypointsParent has child objects.");
        }
        else
        {
            Debug.LogError("Waypoints Parent not assigned!");
        }
    }
    
    void CalculateIdealPathLength()
    {
        if (waypoints.Count < 2) return;
        float length = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
            length += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        
        // Add distance from last waypoint back to first for loop
        if (waypoints.Count > 2)
            length += Vector3.Distance(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            
        idealPathLength = length;
        Debug.Log($"Ideal path length: {idealPathLength:F2} meters");
    }

    void Update()
    {
        if (ShouldStop()) return;
        
        runningTime += Time.deltaTime;
        frameCounter++;
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetPosition = LookAheadWaypoints();
        Vector3 direction = (targetPosition - transform.position).normalized;
        float steeringAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        targetSteerAngle = Mathf.Clamp(steeringAngle, -45f, 45f);
        
        TrackSteeringMetrics(targetSteerAngle);

        // Waypoint lookahead skip logic
        if (currentWaypointIndex < waypoints.Count - 1)
        {
            float distToCurrent = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
            float distToNext = Vector3.Distance(transform.position, waypoints[currentWaypointIndex + 1].position);
            if (distToNext < distToCurrent)
            {
                currentWaypointIndex++;
                Debug.Log($"Skipped to waypoint {currentWaypointIndex}");
                return;
            }
        }

        // NEW LOOP LOGIC - Handle waypoint completion with looping
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        {
            // Check if we're at waypoint 0 (start/finish line)
            if (currentWaypointIndex == 0 && hasStartedFirstLap)
            {
                // Completed a lap!
                CompleteLap();
            }
            else if (currentWaypointIndex == 0 && !hasStartedFirstLap)
            {
                // Starting first lap
                StartFirstLap();
            }

            // Move to next waypoint (with looping)
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Loop back to start
            }
            
            
            
            // Check if we've completed target number of laps
            if (completedLaps >= targetLaps)
            {
                FinishAllLaps();
            }
        }
    }

    // NEW METHODS for lap handling
    void StartFirstLap()
    {
        hasStartedFirstLap = true;
        currentLap = 1;
        lapStartTime = Time.time;
        Debug.Log($"Started Lap {currentLap}");
        
        if (performanceLogger != null)
        {
            performanceLogger.agentType = $"Heuristic_Trial{currentTrialNumber}_Lap{currentLap}";
        }
    }

    void CompleteLap()
    {
        if (!hasStartedFirstLap) return;
        
        float lapTime = Time.time - lapStartTime;
        lapTimes.Add(lapTime);
        completedLaps++;
        
        Debug.Log($"=== LAP {currentLap} COMPLETED ===");
        Debug.Log($"Lap Time: {lapTime:F2} seconds");
        Debug.Log($"Completed Laps: {completedLaps}/{targetLaps}");
        
        // Log lap data to performance logger
        if (performanceLogger != null)
        {
            performanceLogger.RecordLapCompletion(currentLap, lapTime);
        }
        
        // Start next lap if not finished
        if (completedLaps < targetLaps)
        {
            currentLap++;
            lapStartTime = Time.time;
            Debug.Log($"Started Lap {currentLap}");
            
            if (performanceLogger != null)
            {
                performanceLogger.agentType = $"Heuristic_Trial{currentTrialNumber}_Lap{currentLap}";
            }
        }
    }

    void FinishAllLaps()
    {
        Debug.Log("=== ALL LAPS COMPLETED ===");
        LogLapSummary();
        
        if (performanceLogger != null)
        {
            performanceLogger.SetFinished();
        }
        
        // Stop the kart
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Auto-restart if enabled
        if (autoRestartOnCompletion && currentTrialNumber < numberOfTrials)
        {
            StartCoroutine(PrepareNextTrial());
        }
    }

    void LogLapSummary()
    {
        Debug.Log("=== LAP SUMMARY ===");
        for (int i = 0; i < lapTimes.Count; i++)
        {
            Debug.Log($"Lap {i + 1}: {lapTimes[i]:F2}s");
        }
        
        if (lapTimes.Count > 0)
        {
            float bestLap = lapTimes.Min();
            float averageLap = lapTimes.Average();
            Debug.Log($"Best Lap: {bestLap:F2}s");
            Debug.Log($"Average Lap: {averageLap:F2}s");
            Debug.Log($"Total Time: {lapTimes.Sum():F2}s");
        }
        Debug.Log("==================");
    }

    void ResetLapData()
    {
        currentLap = 0;
        completedLaps = 0;
        lapStartTime = 0f;
        lapTimes.Clear();
        hasStartedFirstLap = false;
    }
    
    IEnumerator PrepareNextTrial()
    {
        float previousTimeScale = Time.timeScale;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        
        yield return new WaitForSeconds(delayBetweenTrials);
        currentTrialNumber++;
        Debug.Log($"Starting Trial {currentTrialNumber} of {numberOfTrials}");
        
        // Reset kart position and physics state
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        
        // Reset tracking variables
        currentWaypointIndex = 0;
        reachedEnd = false;
        totalTravelDistance = 0f;
        pathDeviation = 0f;
        frameCounter = 0;
        runningTime = 0f;
        averageSteeringAngle = 0f;
        steeringAngleSamples = 0;
        maxSteeringAngle = 0f;
        timeSpentBraking = 0f;
        lastPosition = transform.position;
        startupPhase = true;
        startupTimer = 0f;
        
        // Reset lap data
        ResetLapData();
        
        // Reset the logger
        if (performanceLogger != null)
        {
            performanceLogger.ResetMetrics();
            performanceLogger.agentType = $"Heuristic_Trial{currentTrialNumber}";
        }
        else
        {
            performanceLogger = gameObject.AddComponent<AIPerformanceLogger>();
            performanceLogger.agentType = $"Heuristic_Trial{currentTrialNumber}";
        }
        
        StartCoroutine(SmoothStart());
        SetSimulationSpeed(previousTimeScale);
    }
    
    Vector3 LookAheadWaypoints()
    {
        if (waypoints.Count == 0) return transform.position + transform.forward;
        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        float distanceSum = 0f;
        int lookAheadIndex = currentWaypointIndex;
        
        while (distanceSum < lookAheadDistance)
        {
            int nextIndex = (lookAheadIndex + 1) % waypoints.Count; // Handle looping
            distanceSum += Vector3.Distance(waypoints[lookAheadIndex].position, waypoints[nextIndex].position);
            lookAheadIndex = nextIndex;
            
            // Prevent infinite loop
            if (lookAheadIndex == currentWaypointIndex) break;
        }
        
        if (lookAheadIndex != currentWaypointIndex)
        {
            float blendFactor = 0.7f;
            targetPos = Vector3.Lerp(
                waypoints[currentWaypointIndex].position,
                waypoints[lookAheadIndex].position, 
                blendFactor);
        }
        return targetPos;
    }
    
    void FixedUpdate()
    {
        if (ShouldStop()) return;
        
        if (!startupPhase)
        {
            ApplySteeringForce();
            ApplyDriveForce();
            ApplyLateralFriction();
        }
        
        TrackDistanceMetrics();
        
        if (performanceLogger != null)
        {
            string phase = startupPhase ? "startup" : "driving";
            performanceLogger.RecordTrajectorySample(
                Time.time, transform.position, rb.velocity, targetSteerAngle, 0f, phase
            );
        }
    }
    
    void ApplySteeringForce()
    {
        float steerForce = targetSteerAngle * steeringResponse;
        rb.AddTorque(Vector3.up * steerForce, ForceMode.Acceleration);
    }
    
    void ApplyDriveForce()
    {
        float forwardVelocity = Vector3.Dot(rb.velocity, transform.forward);
        float speedFactor = 1.0f - (Mathf.Abs(targetSteerAngle) / 45f * 0.3f);
        float targetSpeed = baseSpeed * speedFactor;
        
        Vector3 forcePos = transform.position;
        forcePos.y -= 0.5f;
        
        if (forwardVelocity < targetSpeed)
        {
            float accelerationFactor = Mathf.Clamp01(1.0f - (forwardVelocity / targetSpeed));
            rb.AddForceAtPosition(transform.forward * accelerationForce * accelerationFactor, 
                                 forcePos, ForceMode.Acceleration);
            
            if (forwardVelocity < 0.5f)
            {
                rb.AddForce(transform.forward * accelerationForce * 0.3f, ForceMode.Acceleration);
            }
        }
        else
        {
            float brakeFactor = (forwardVelocity - targetSpeed) / targetSpeed;
            rb.AddForce(-transform.forward * brakingForce * brakeFactor, ForceMode.Acceleration);
            timeSpentBraking += Time.fixedDeltaTime;
        }
    }
    
    void ApplyLateralFriction()
    {
        Vector3 lateralVelocity = Vector3.Project(rb.velocity, transform.right);
        rb.AddForce(-lateralVelocity * lateralFriction, ForceMode.Acceleration);
    }
    
    void TrackDistanceMetrics()
    {
        float segmentDistance = Vector3.Distance(transform.position, lastPosition);
        totalTravelDistance += segmentDistance;
        if (currentWaypointIndex > 0 && currentWaypointIndex < waypoints.Count)
        {
            Vector3 previousWaypoint = waypoints[currentWaypointIndex - 1].position;
            Vector3 currentWaypoint = waypoints[currentWaypointIndex].position;
            Vector3 idealSegment = currentWaypoint - previousWaypoint;
            Vector3 projectedPos = previousWaypoint + Vector3.Project(
                transform.position - previousWaypoint, 
                idealSegment.normalized);
            float deviation = Vector3.Distance(transform.position, projectedPos);
            pathDeviation += deviation;
        }
        lastPosition = transform.position;
    }
    
    void TrackSteeringMetrics(float steerAngle)
    {
        averageSteeringAngle = ((averageSteeringAngle * steeringAngleSamples) + Mathf.Abs(steerAngle)) / (steeringAngleSamples + 1);
        steeringAngleSamples++;
        if (Mathf.Abs(steerAngle) > maxSteeringAngle)
            maxSteeringAngle = Mathf.Abs(steerAngle);
    }
    
    void StopKartOnSlope()
    {
        Debug.Log("Reached end of track - stopping kart");
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        LogDetailedMetrics();
    }
    
    void LogDetailedMetrics()
    {
        Debug.Log("=== DETAILED HEURISTIC METRICS ===");
        Debug.Log($"Trial: {currentTrialNumber} of {numberOfTrials}");
        Debug.Log($"Total Distance: {totalTravelDistance:F2}m");
        Debug.Log($"Ideal Path Length: {idealPathLength:F2}m");
        Debug.Log($"Path Efficiency: {idealPathLength / totalTravelDistance:P2}");
        Debug.Log($"Average Steering Angle: {averageSteeringAngle:F2}°");
        Debug.Log($"Max Steering Angle: {maxSteeringAngle:F2}°");
        Debug.Log($"Time Spent Braking: {timeSpentBraking:F2}s ({timeSpentBraking/runningTime:P2})");
        Debug.Log($"Average Speed: {totalTravelDistance/runningTime:F2} m/s");
        Debug.Log($"Frame Rate: {frameCounter/runningTime:F2} FPS");
        Debug.Log("================================");
    }
    
    bool ShouldStop()
    {
        return waypoints == null || waypoints.Count == 0 || completedLaps >= targetLaps;
    }
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !showWaypointGizmos) return;
        if (waypoints != null && waypoints.Count > 0 && currentWaypointIndex < waypoints.Count)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
            Gizmos.DrawWireSphere(waypoints[currentWaypointIndex].position, waypointThreshold);
            Gizmos.color = Color.yellow;
            Vector3 lookAheadPos = LookAheadWaypoints();
            Gizmos.DrawWireSphere(lookAheadPos, 0.5f);
        }
        if (showDebugInfo)
        {
            Gizmos.color = Color.blue;
            Vector3 steeringDir = Quaternion.Euler(0, targetSteerAngle, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, steeringDir * 3f);
        }
    }
    
    public void StartAutomatedTesting()
    {
        autoRestartOnCompletion = true;
        currentTrialNumber = 1;
        
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        
        currentWaypointIndex = 0;
        reachedEnd = false;
        totalTravelDistance = 0f;
        pathDeviation = 0f;
        frameCounter = 0;
        runningTime = 0f;
        averageSteeringAngle = 0f;
        steeringAngleSamples = 0;
        maxSteeringAngle = 0f;
        timeSpentBraking = 0f;
        lastPosition = transform.position;
        startupPhase = true;
        startupTimer = 0f;
        
        // Reset lap data
        ResetLapData();
        
        if (performanceLogger != null)
        {
            Destroy(performanceLogger);
        }
        performanceLogger = gameObject.AddComponent<AIPerformanceLogger>();
        performanceLogger.agentType = $"Heuristic_Trial{currentTrialNumber}";
        
        StartCoroutine(SmoothStart());
        
        Debug.Log($"Starting automated testing with {numberOfTrials} trials");
    }
    
    public void SetSimulationSpeedFactor(float factor)
    {
        SetSimulationSpeed(factor);
    }
}
