using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

public class AIPerformanceLogger : MonoBehaviour
{
    [Header("Configuration")]
    public string agentType = "Heuristic";
    [Tooltip("Leave blank to use default path")]
    public string logFilePath = "";
    public bool enableLogging = true;
    public bool saveDetailedTimeSeries = true;

    [Header("Metrics")]
    private float startTime;
    private float completionTime;
    private int collisionCount = 0;
    private float distanceTraveled = 0f;
    private List<float> speedSamples = new List<float>();
    private List<float> pathDeviationSamples = new List<float>();
    private List<float> steeringAngleSamples = new List<float>();
    private List<float> accelerationSamples = new List<float>();
    private Vector3 lastPosition;
    private Vector3 lastVelocity;
    private List<TimeSeriesDataPoint> timeSeriesData = new List<TimeSeriesDataPoint>();
    private List<CheckpointData> checkpointData = new List<CheckpointData>();
    private List<LapData> lapData = new List<LapData>();
    private Rigidbody rb;
    private bool hasFinished = false;
    private float sampleInterval = 0.1f;
    private float timeSinceLastSample = 0f;
    private string actualLogPath;

    // Per-lap accumulators
    private List<float> lapSpeedSamples = new List<float>();
    private List<float> lapSteeringSamples = new List<float>();
    private List<float> lapAccelerationSamples = new List<float>();
    private List<float> lapPathDeviationSamples = new List<float>();
    private int lapCollisionCount = 0;
    private float lapStartTimestamp = 0f;

    [System.Serializable]
    private class TimeSeriesDataPoint
    {
        public float time;
        public Vector3 position;
        public Vector3 velocity;
        public float speed;
        public float steeringAngle;
        public float acceleration;
        public float pathDeviation;
        public string phase;
    }

    [System.Serializable]
    private class CheckpointData
    {
        public int checkpointID;
        public string segmentName;
        public bool isSegmentStart;
        public bool isSegmentEnd;
        public float segmentTime;
        public float timestamp;
        public Vector3 position;
        public float speed;
    }

    [System.Serializable]
    private class LapData
    {
        public int lapNumber;
        public float lapTime;
        public float startTimestamp;
        public float endTimestamp;
        public float averageSpeed;
        public float maxSpeed;
        public float minSpeed;
        public float averageSteering;
        public float maxSteering;
        public float averageAcceleration;
        public float maxAcceleration;
        public float totalPathDeviation;
        public int collisionCount;
        public float lapEfficiency;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;
        lastPosition = transform.position;
        lastVelocity = rb != null ? rb.velocity : Vector3.zero;
        lapStartTimestamp = 0f;
        SetupLoggingDirectory();
    }

    void SetupLoggingDirectory()
    {
        if (string.IsNullOrEmpty(logFilePath))
            actualLogPath = Path.Combine(Application.persistentDataPath, "AIPerformanceLogs");
        else
            actualLogPath = logFilePath;

        if (!Directory.Exists(actualLogPath))
            Directory.CreateDirectory(actualLogPath);

        Debug.Log($"Performance logging initialized. Data will be saved to: {actualLogPath}");
    }

    public void LogExportPath()
    {
        Debug.Log("=== CSV EXPORT LOCATION ===");
        Debug.Log($"CSV files will be exported to: {actualLogPath}");
        Debug.Log($"Full path: {Path.GetFullPath(actualLogPath)}");
        Debug.Log("==========================");
    }

    void FixedUpdate()
    {
        if (hasFinished || !enableLogging || rb == null) return;
        float segmentDistance = Vector3.Distance(transform.position, lastPosition);
        distanceTraveled += segmentDistance;
        Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
        float accelerationMagnitude = acceleration.magnitude;
        timeSinceLastSample += Time.fixedDeltaTime;
        if (timeSinceLastSample >= sampleInterval)
        {
            float speed = rb.velocity.magnitude;
            speedSamples.Add(speed);
            lapSpeedSamples.Add(speed);

            float steeringAngle = Vector3.SignedAngle(lastVelocity.normalized, rb.velocity.normalized, Vector3.up);
            steeringAngleSamples.Add(Mathf.Abs(steeringAngle));
            lapSteeringSamples.Add(Mathf.Abs(steeringAngle));

            accelerationSamples.Add(accelerationMagnitude);
            lapAccelerationSamples.Add(accelerationMagnitude);

            float deviation = CalculatePathDeviation();
            pathDeviationSamples.Add(deviation);
            lapPathDeviationSamples.Add(deviation);

            if (saveDetailedTimeSeries)
            {
                TimeSeriesDataPoint dataPoint = new TimeSeriesDataPoint
                {
                    time = Time.time - startTime,
                    position = transform.position,
                    velocity = rb.velocity,
                    speed = speed,
                    steeringAngle = steeringAngle,
                    acceleration = accelerationMagnitude,
                    pathDeviation = deviation,
                    phase = "driving"
                };
                timeSeriesData.Add(dataPoint);
            }
            timeSinceLastSample = 0f;
        }
        lastPosition = transform.position;
        lastVelocity = rb.velocity;
    }

    public void SetFinished()
    {
        if (hasFinished || !enableLogging) return;
        completionTime = Time.time - startTime;
        hasFinished = true;
        SaveResults();
    }

    public void RecordCollision()
    {
        collisionCount++;
        lapCollisionCount++;
        Debug.Log($"Collision detected: {collisionCount}");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            RecordCollision();
    }

    float CalculatePathDeviation()
    {
        // Implement your own path deviation logic if needed
        return 0.0f;
    }

    public void RecordCheckpoint(int checkpointID, string segmentName, bool isSegmentStart, bool isSegmentEnd, float segmentTime)
    {
        if (!enableLogging || rb == null) return;

        CheckpointData data = new CheckpointData
        {
            checkpointID = checkpointID,
            segmentName = segmentName,
            isSegmentStart = isSegmentStart,
            isSegmentEnd = isSegmentEnd,
            segmentTime = segmentTime,
            timestamp = Time.time - startTime,
            position = transform.position,
            speed = rb.velocity.magnitude
        };

        checkpointData.Add(data);

        if (isSegmentEnd && segmentTime > 0)
        {
            Debug.Log($"Segment '{segmentName}' completed in {segmentTime:F2} seconds at speed {rb.velocity.magnitude:F2} m/s");
        }
    }

    // Record lap completion with detailed metrics
    public void RecordLapCompletion(int lapNumber, float lapTime)
    {
        if (!enableLogging) return;

        float averageSpeed = lapSpeedSamples.Count > 0 ? lapSpeedSamples.Average() : 0f;
        float maxSpeed = lapSpeedSamples.Count > 0 ? lapSpeedSamples.Max() : 0f;
        float minSpeed = lapSpeedSamples.Count > 0 ? lapSpeedSamples.Min() : 0f;
        float averageSteering = lapSteeringSamples.Count > 0 ? lapSteeringSamples.Average() : 0f;
        float maxSteering = lapSteeringSamples.Count > 0 ? lapSteeringSamples.Max() : 0f;
        float averageAcceleration = lapAccelerationSamples.Count > 0 ? lapAccelerationSamples.Average() : 0f;
        float maxAcceleration = lapAccelerationSamples.Count > 0 ? lapAccelerationSamples.Max() : 0f;
        float totalPathDeviation = lapPathDeviationSamples.Sum();
        float lapEfficiency = (distanceTraveled > 0f) ? (distanceTraveled - lapPathDeviationSamples.Sum()) / distanceTraveled : 0f;

        LapData lap = new LapData
        {
            lapNumber = lapNumber,
            lapTime = lapTime,
            startTimestamp = lapStartTimestamp,
            endTimestamp = Time.time - startTime,
            averageSpeed = averageSpeed,
            maxSpeed = maxSpeed,
            minSpeed = minSpeed,
            averageSteering = averageSteering,
            maxSteering = maxSteering,
            averageAcceleration = averageAcceleration,
            maxAcceleration = maxAcceleration,
            totalPathDeviation = totalPathDeviation,
            collisionCount = lapCollisionCount,
            lapEfficiency = lapEfficiency
        };

        lapData.Add(lap);
        Debug.Log($"Performance Logger: Lap {lapNumber} completed in {lapTime:F2} seconds");

        // Reset per-lap accumulators for next lap
        lapSpeedSamples.Clear();
        lapSteeringSamples.Clear();
        lapAccelerationSamples.Clear();
        lapPathDeviationSamples.Clear();
        lapCollisionCount = 0;
        lapStartTimestamp = Time.time - startTime;
    }

    public void RecordTrajectorySample(float time, Vector3 position, Vector3 velocity, float steeringAngle, float curvature, string phase)
    {
        if (!enableLogging) return;

        float adjustedTime = time - startTime;

        TimeSeriesDataPoint dataPoint = new TimeSeriesDataPoint
        {
            time = adjustedTime,
            position = position,
            velocity = velocity,
            speed = velocity.magnitude,
            steeringAngle = steeringAngle,
            acceleration = 0f,
            pathDeviation = 0f,
            phase = phase
        };

        timeSeriesData.Add(dataPoint);
    }

    void SaveResults()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = Path.Combine(actualLogPath, $"AI_{agentType}_Run_{timestamp}.csv");
        string timeSeriesFilename = Path.Combine(actualLogPath, $"AI_{agentType}_TimeSeries_{timestamp}.csv");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Metric,Value");
        sb.AppendLine($"Agent Type,{agentType}");
        sb.AppendLine($"Completion Time (s),{completionTime}");
        sb.AppendLine($"Collision Count,{collisionCount}");
        sb.AppendLine($"Distance Traveled (m),{distanceTraveled}");

        float avgSpeed = speedSamples.Count > 0 ? speedSamples.Average() : 0f;
        if (speedSamples.Count > 0)
        {
            float maxSpeed = speedSamples.Max();
            float minSpeed = speedSamples.Min();
            float speedStdDev = CalculateStandardDeviation(speedSamples);
            sb.AppendLine($"Average Speed (m/s),{avgSpeed}");
            sb.AppendLine($"Maximum Speed (m/s),{maxSpeed}");
            sb.AppendLine($"Minimum Speed (m/s),{minSpeed}");
            sb.AppendLine($"Speed Standard Deviation,{speedStdDev}");
            sb.AppendLine($"Speed Consistency,{(avgSpeed != 0f ? 1.0f - (speedStdDev / avgSpeed) : 0f)}");
        }

        if (steeringAngleSamples.Count > 0)
        {
            float avgSteering = steeringAngleSamples.Average();
            float maxSteering = steeringAngleSamples.Max();
            sb.AppendLine($"Average Steering Angle (deg),{avgSteering}");
            sb.AppendLine($"Maximum Steering Angle (deg),{maxSteering}");
            sb.AppendLine($"Steering Smoothness,{1.0f - (avgSteering / 180.0f)}");
        }

        if (accelerationSamples.Count > 0)
        {
            float avgAccel = accelerationSamples.Average();
            float maxAccel = accelerationSamples.Max();
            sb.AppendLine($"Average Acceleration (m/s²),{avgAccel}");
            sb.AppendLine($"Maximum Acceleration (m/s²),{maxAccel}");
        }

        if (pathDeviationSamples.Count > 0)
        {
            sb.AppendLine($"Average Path Deviation (m),{pathDeviationSamples.Average()}");
            sb.AppendLine($"Maximum Path Deviation (m),{pathDeviationSamples.Max()}");
        }

        // Add lap data to the CSV
        if (lapData.Count > 0)
        {
            sb.AppendLine("\nLap Data");
            sb.AppendLine("Lap,StartTime,EndTime,LapTime,AvgSpeed,MaxSpeed,MinSpeed,AvgSteering,MaxSteering,AvgAccel,MaxAccel,PathDeviation,Collisions,Efficiency");
            foreach (var lap in lapData)
            {
                sb.AppendLine($"{lap.lapNumber},{lap.startTimestamp:F3},{lap.endTimestamp:F3},{lap.lapTime:F3},{lap.averageSpeed:F3},{lap.maxSpeed:F3},{lap.minSpeed:F3},{lap.averageSteering:F3},{lap.maxSteering:F3},{lap.averageAcceleration:F3},{lap.maxAcceleration:F3},{lap.totalPathDeviation:F3},{lap.collisionCount},{lap.lapEfficiency:F3}");
            }

            // Lap summary statistics
            if (lapData.Count > 1)
            {
                sb.AppendLine("\nLap Summary");
                sb.AppendLine("Metric,Value");
                sb.AppendLine($"Total Laps,{lapData.Count}");
                sb.AppendLine($"Best Lap Time,{lapData.Min(l => l.lapTime):F3}");
                sb.AppendLine($"Worst Lap Time,{lapData.Max(l => l.lapTime):F3}");
                sb.AppendLine($"Average Lap Time,{lapData.Average(l => l.lapTime):F3}");
                sb.AppendLine($"Lap Time Consistency,{1.0f - (CalculateStandardDeviation(lapData.Select(l => l.lapTime).ToList()) / lapData.Average(l => l.lapTime)):F3}");
            }
        }

        // Add checkpoint segment data to the main CSV
        if (checkpointData.Count > 0)
        {
            sb.AppendLine("\nCheckpoint Data");
            sb.AppendLine("CheckpointID,SegmentName,IsStart,IsEnd,SegmentTime,Timestamp,Speed");

            foreach (var checkpoint in checkpointData)
            {
                sb.AppendLine($"{checkpoint.checkpointID},{checkpoint.segmentName},{checkpoint.isSegmentStart}," +
                             $"{checkpoint.isSegmentEnd},{checkpoint.segmentTime:F3},{checkpoint.timestamp:F3},{checkpoint.speed:F3}");
            }

            var segments = checkpointData.Where(c => c.isSegmentEnd && c.segmentTime > 0)
                                        .GroupBy(c => c.segmentName)
                                        .Select(g => new {
                                            SegmentName = g.Key,
                                            AverageTime = g.Average(c => c.segmentTime),
                                            MinTime = g.Min(c => c.segmentTime),
                                            MaxTime = g.Max(c => c.segmentTime),
                                            Count = g.Count()
                                        });

            if (segments.Any())
            {
                sb.AppendLine("\nSegment Summary");
                sb.AppendLine("SegmentName,AverageTime,MinTime,MaxTime,Count");

                foreach (var segment in segments)
                {
                    sb.AppendLine($"{segment.SegmentName},{segment.AverageTime:F3},{segment.MinTime:F3}," +
                                 $"{segment.MaxTime:F3},{segment.Count}");
                }
            }
        }

        if (avgSpeed > 0)
        {
            float idealTime = distanceTraveled / avgSpeed;
            float efficiency = idealTime / completionTime;
            sb.AppendLine($"Time Efficiency,{efficiency}");
        }

        File.WriteAllText(filename, sb.ToString());
        Debug.Log($"Saved performance summary to: {filename}");
        Debug.Log($"Full path: {Path.GetFullPath(filename)}");

        if (saveDetailedTimeSeries && timeSeriesData.Count > 0)
        {
            SaveTimeSeriesData(timeSeriesFilename);
        }
    }

    void SaveTimeSeriesData(string filename)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Time,PosX,PosY,PosZ,VelX,VelY,VelZ,Speed,SteeringAngle,Acceleration,PathDeviation,Phase,CheckpointID");

        Dictionary<float, int> checkpointLookup = new Dictionary<float, int>();
        foreach (var checkpoint in checkpointData)
        {
            checkpointLookup[checkpoint.timestamp] = checkpoint.checkpointID;
        }

        foreach (var point in timeSeriesData)
        {
            int checkpointID = -1;
            foreach (var checkpoint in checkpointData)
            {
                if (Mathf.Abs(point.time - checkpoint.timestamp) < 0.1f)
                {
                    checkpointID = checkpoint.checkpointID;
                    break;
                }
            }

            sb.AppendLine($"{point.time:F3}," +
                          $"{point.position.x:F3},{point.position.y:F3},{point.position.z:F3}," +
                          $"{point.velocity.x:F3},{point.velocity.y:F3},{point.velocity.z:F3}," +
                          $"{point.speed:F3},{point.steeringAngle:F3},{point.acceleration:F3},{point.pathDeviation:F3},{point.phase},{checkpointID}");
        }

        File.WriteAllText(filename, sb.ToString());
        Debug.Log($"Saved detailed time series data to: {filename}");
        Debug.Log($"Full path: {Path.GetFullPath(filename)}");
    }

    float CalculateStandardDeviation(List<float> values)
    {
        if (values.Count <= 1) return 0;
        float avg = values.Average();
        float sum = values.Sum(v => (v - avg) * (v - avg));
        return Mathf.Sqrt(sum / (values.Count - 1));
    }

    public void RecordSteeringInput(float angle)
    {
        if (!enableLogging) return;
        steeringAngleSamples.Add(Mathf.Abs(angle));
    }

    public void RecordAcceleration(Vector3 accel)
    {
        if (!enableLogging) return;
        accelerationSamples.Add(accel.magnitude);
    }

    public void RecordWaypointPass(int waypointIndex, float segmentTime)
    {
        if (!enableLogging) return;
        Debug.Log($"Waypoint {waypointIndex} passed in {segmentTime:F2} seconds");
    }

    public void RecordWaypointSegments(List<float> segmentTimes)
    {
        if (!enableLogging || segmentTimes == null || segmentTimes.Count == 0) return;

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = Path.Combine(actualLogPath, $"AI_{agentType}_WaypointSegments_{timestamp}.csv");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("SegmentIndex,Time(s)");

        for (int i = 0; i < segmentTimes.Count; i++)
        {
            sb.AppendLine($"{i},{segmentTimes[i]:F3}");
        }

        File.WriteAllText(filename, sb.ToString());
        Debug.Log($"Saved waypoint segment times to: {filename}");
    }

    public void ResetMetrics()
    {
        if (!hasFinished && enableLogging)
        {
            completionTime = Time.time - startTime;
            SaveResults();
        }

        startTime = Time.time;
        completionTime = 0f;
        collisionCount = 0;
        distanceTraveled = 0f;
        speedSamples.Clear();
        pathDeviationSamples.Clear();
        steeringAngleSamples.Clear();
        accelerationSamples.Clear();
        timeSeriesData.Clear();
        checkpointData.Clear();
        lapData.Clear();
        hasFinished = false;
        timeSinceLastSample = 0f;

        lastPosition = transform.position;
        lastVelocity = rb != null ? rb.velocity : Vector3.zero;

        // Reset per-lap accumulators
        lapSpeedSamples.Clear();
        lapSteeringSamples.Clear();
        lapAccelerationSamples.Clear();
        lapPathDeviationSamples.Clear();
        lapCollisionCount = 0;
        lapStartTimestamp = 0f;

        Debug.Log("Performance metrics reset for new trial");
    }
}
