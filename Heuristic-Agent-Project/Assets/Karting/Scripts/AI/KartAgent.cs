using KartGame.KartSystems;

using Unity.MLAgents;

using Unity.MLAgents.Sensors;

using Unity.MLAgents.Actuators;

using UnityEngine;

using Random = UnityEngine.Random;

using System.IO;



namespace KartGame.AI

{

    [System.Serializable]

    public struct Sensor

    {

        public Transform Transform;

        public float RayDistance;

        public float HitValidationDistance;

    }



    public enum AgentMode

    {

        Training,

        Inferencing

    }



    public class KartAgent : Agent, IInput

    {

        #region Training Modes

        [Tooltip("Are we training the agent or is the agent production ready?")]

        public AgentMode Mode = AgentMode.Training;

        [Tooltip("What is the initial checkpoint the agent will go to? This value is only for inferencing.")]

        public ushort InitCheckpointIndex;

        #endregion



        #region Senses

        [Header("Observation Params")]

        [Tooltip("What objects should the raycasts hit and detect?")]

        public LayerMask Mask;

        [Tooltip("Sensors contain ray information to sense out the world, you can have as many sensors as you need.")]

        public Sensor[] Sensors;

        [Header("Checkpoints"), Tooltip("What are the series of checkpoints for the agent to seek and pass through?")]

        public Collider[] Colliders;

        [Tooltip("What layer are the checkpoints on? This should be an exclusive layer for the agent to use.")]

        public LayerMask CheckpointMask;



        [Space]

        [Tooltip("Would the agent need a custom transform to be able to raycast and hit the track? " +

            "If not assigned, then the root transform will be used.")]

        public Transform AgentSensorTransform;

        #endregion



        #region Rewards

        [Header("Basic Rewards")]

        public float HitPenalty = -1f;

        public float PassCheckpointReward = 1f;

        public float LapCompletionReward = 10f;

        public float WrongDirectionPenalty = -0.5f;

        public float TowardsCheckpointReward = 0.03f;

        public float SpeedReward = 0.02f;

        public float AccelerationReward = 0.01f;



        [Header("New Advanced Rewards")]

        public float CenterLineReward = 0.05f;

        public float SmoothSteeringPenalty = -0.02f;

        public float OptimalSpeedReward = 0.03f;

        public float MinSpeedThreshold = 3f;

        public float MaxSpeedThreshold = 15f;

        public float SlowSpeedPenalty = -0.01f;

        public float ProgressReward = 0.02f;

        public float TrackWidth = 10f;

        #endregion



        #region ResetParams

        [Header("Inference Reset Params")]

        public LayerMask OutOfBoundsMask;

        public LayerMask TrackMask;

        public float GroundCastDistance;

        #endregion



        #region Debugging

        [Header("Debug Options")]

        public bool ShowRaycasts;

        public bool EnableDebugLogging = true;

        public int LogFrequency = 100;

        #endregion



        #region CSV Logging

        [Header("CSV Logging")]

        public bool EnableCSVLogging = true;

        public string CSVFileName = "MLAgentResults.csv";

        #endregion



        private float m_TargetSteering;

        public float SteeringSharpness = 10f;

        ArcadeKart m_Kart;

        bool m_Acceleration;

        bool m_Brake;

        float m_Steering;

        int m_CheckpointIndex;



        bool m_EndEpisode;

        float m_LastAccumulatedReward;



        // Debug tracking variables

        private int m_KartStepCount = 0;

        private int m_WallHitCount = 0;

        private int m_CheckpointsPassed = 0;

        private int m_CheckpointsPassedThisLap = 0;

        private int m_LapsCompleted = 0;

        private float m_TotalReward = 0f;

        private float m_EpisodeStartTime = 0f;

        private float m_LapStartTime = 0f;

        private float m_LastLapTime = 0f;

        private Vector3 m_LastPosition;

        private float m_WrongDirectionTimer = 0f;



        private float m_LastSteering = 0f;

        private Vector3 m_LastPositionForProgress;

        private float m_LastDistanceToNextCheckpoint = float.MaxValue;



        // CSV logging variables

        private static string csvPath = "";

        private static bool csvHeaderWritten = false;

        private int m_EpisodeCount = 0;

        private float m_TotalDistanceTraveled = 0f;

        private float m_MaxSpeedThisEpisode = 0f;

        private float m_MinSpeedThisEpisode = float.MaxValue;

        private float m_SpeedSum = 0f;

        private int m_SpeedSamples = 0;



        private double m_CumulativeTrainingSeconds = 21300; // Start from your current total

        private string trainingTimePath = "";



        void Awake()



        {

            m_Kart = GetComponent<ArcadeKart>();

            if (AgentSensorTransform == null) AgentSensorTransform = transform;



            {

    m_Kart = GetComponent<ArcadeKart>();

    if (AgentSensorTransform == null) AgentSensorTransform = transform;



    // Save to Desktop for visibility

    string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

    trainingTimePath = Path.Combine(desktopPath, "TrainingTime.txt");



    // Load from file if it exists

    if (File.Exists(trainingTimePath))

    {

        string content = File.ReadAllText(trainingTimePath);

        double loadedSeconds;

        if (double.TryParse(content, out loadedSeconds))

        {

            m_CumulativeTrainingSeconds = loadedSeconds;

            Debug.Log($"[TRAINING TIME] Loaded cumulative time: {m_CumulativeTrainingSeconds} seconds");

        }

    }

    else

    {

        // Save initial value

        File.WriteAllText(trainingTimePath, m_CumulativeTrainingSeconds.ToString());

    }

}

        }

        void Start()

        {

            OnEpisodeBegin();

            if (Mode == AgentMode.Inferencing) m_CheckpointIndex = InitCheckpointIndex;

        }



        void Update()

        {

           

            {

                m_Steering = Mathf.MoveTowards(m_Steering, m_TargetSteering, Time.deltaTime * SteeringSharpness);



    if (m_EndEpisode)

                {

                    m_EndEpisode = false;

                    AddReward(m_LastAccumulatedReward);



                    // Calculate episode duration

                    float episodeDuration = Time.time - m_EpisodeStartTime;

                    m_CumulativeTrainingSeconds += episodeDuration;



                    // Save cumulative time to file

                    try

                    {

                        File.WriteAllText(trainingTimePath, m_CumulativeTrainingSeconds.ToString());

                        Debug.Log($"[TRAINING TIME] Updated cumulative time: {m_CumulativeTrainingSeconds} seconds (~{(m_CumulativeTrainingSeconds / 3600):F2} hours)");

                    }

                    catch (System.Exception e)

                    {

                        Debug.LogError($"[TRAINING TIME ERROR] Failed to write training time: {e.Message}");

                    }



                    if (EnableCSVLogging)

                    {

                        m_EpisodeCount++;

                        LogEpisodeToCSV();

                    }



                    if (EnableDebugLogging)

                    {

                        Debug.Log($"[EPISODE END] Agent: {gameObject.name} | Duration: {episodeDuration:F2}s | Total Training Time: {m_CumulativeTrainingSeconds:F2}s (~{(m_CumulativeTrainingSeconds / 3600):F2}h)");

                    }



                    EndEpisode();

                    OnEpisodeBegin();

                }

}

            if (EnableCSVLogging)

            {

                float currentSpeed = m_Kart.LocalSpeed();

                m_SpeedSum += currentSpeed;

                m_SpeedSamples++;

                m_MaxSpeedThisEpisode = Mathf.Max(m_MaxSpeedThisEpisode, currentSpeed);

                m_MinSpeedThisEpisode = Mathf.Min(m_MinSpeedThisEpisode, currentSpeed);



                if (m_LastPosition != Vector3.zero)

                {

                    m_TotalDistanceTraveled += Vector3.Distance(transform.position, m_LastPosition);

                }

                m_LastPosition = transform.position;

            }

            {

    if (Time.timeScale != 1.0f)

    {

        Debug.Log($"TimeScale is {Time.timeScale} - should be 1.0");

        Time.timeScale = 1.0f; // Reset to normal

    }

}



            CheckWrongDirection();



            if (m_EndEpisode)

            {

                m_EndEpisode = false;

                AddReward(m_LastAccumulatedReward);



                if (EnableCSVLogging)

                {

                    m_EpisodeCount++;

                    LogEpisodeToCSV();

                }



                if (EnableDebugLogging)

                {

                    float episodeDuration = Time.time - m_EpisodeStartTime;

                    Debug.Log($"[EPISODE END] Agent: {gameObject.name} | Duration: {episodeDuration:F2}s | " +

                             $"Total Reward: {GetCumulativeReward():F3} | Wall Hits: {m_WallHitCount} | " +

                             $"Checkpoints: {m_CheckpointsPassedThisLap}/{Colliders.Length} | Laps: {m_LapsCompleted} | " +

                             $"Last Lap Time: {m_LastLapTime:F2}s | Steps: {m_KartStepCount}");

                }



                EndEpisode();

                OnEpisodeBegin();

            }

        }



        private void LogEpisodeToCSV()

        {

            if (csvPath == "")

            {

                // Save to Desktop

                string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

                csvPath = Path.Combine(desktopPath, CSVFileName);

            }



            if (!csvHeaderWritten)

            {

                string header = "AgentType,Episode,Laps,CheckpointsThisLap,TotalCheckpoints,WallHits,LapTime,TotalReward,AvgSpeed,MaxSpeed,MinSpeed,DistanceTraveled,EpisodeDuration\n";

                File.WriteAllText(csvPath, header);

                csvHeaderWritten = true;

            }



            string agentType = "MLAgent";

            float lapTime = m_LastLapTime > 0 ? m_LastLapTime : (Time.time - m_EpisodeStartTime);

            float avgSpeed = m_SpeedSamples > 0 ? m_SpeedSum / m_SpeedSamples : 0f;

            float episodeDuration = Time.time - m_EpisodeStartTime;



            string line = $"{agentType},{m_EpisodeCount},{m_LapsCompleted},{m_CheckpointsPassedThisLap},{m_CheckpointsPassed}," +

                         $"{m_WallHitCount},{lapTime:F2},{GetCumulativeReward():F3},{avgSpeed:F2},{m_MaxSpeedThisEpisode:F2}," +

                         $"{m_MinSpeedThisEpisode:F2},{m_TotalDistanceTraveled:F2},{episodeDuration:F2}\n";



            File.AppendAllText(csvPath, line);

        }



        void CheckWrongDirection()

        {

            // Get direction to next checkpoint

            var next = (m_CheckpointIndex + 1) % Colliders.Length;

            var nextCheckpoint = Colliders[next];

            if (nextCheckpoint == null) return;



            var directionToNext = (nextCheckpoint.transform.position - transform.position).normalized;

            var velocityDirection = m_Kart.Rigidbody.velocity.normalized;

            var dot = Vector3.Dot(velocityDirection, directionToNext);



            // If moving significantly backwards (dot < -0.5) and has some speed

            if (dot < -0.5f && m_Kart.LocalSpeed() > 2f)

            {

                m_WrongDirectionTimer += Time.deltaTime;

               

                // Apply penalty if going wrong direction for more than 1 second

                if (m_WrongDirectionTimer > 1f)

                {

                    AddReward(WrongDirectionPenalty);

                    m_WrongDirectionTimer = 0f; // Reset timer to avoid constant penalty

                   

                    if (EnableDebugLogging)

                    {

                        Debug.Log($"[WRONG DIRECTION] Agent: {gameObject.name} | Penalty: {WrongDirectionPenalty} | " +

                                 $"Speed: {m_Kart.LocalSpeed():F2} | Dot: {dot:F3}");

                    }

                }

            }

            else

            {

                m_WrongDirectionTimer = 0f; // Reset timer when going correct direction

            }

        }



        // NEW: Calculate centerline reward based on distance from track center

        void AddCenterLineReward()

        {

            // Find the closest LaneDivider object

            GameObject[] centerLineObjects = GameObject.FindGameObjectsWithTag("LaneDivider");

            if (centerLineObjects.Length == 0) return;



            // Find the closest point on the center line

            float minDistance = float.MaxValue;

            Vector3 closestPoint = Vector3.zero;

            foreach (var obj in centerLineObjects)

            {

                // If your CenterLine is a series of points, use their positions

                float dist = Vector3.Distance(transform.position, obj.transform.position);

                if (dist < minDistance)

                {

                    minDistance = dist;

                    closestPoint = obj.transform.position;

                }

            }



            // Lateral distance from center line (project onto track plane if needed)

            float distanceFromCenter = Vector3.Distance(transform.position, closestPoint);



            // Normalize: 0 (centered) to 1 (at edge or worse)

            float normalized = Mathf.Clamp01(distanceFromCenter / (TrackWidth / 2f));



            // Reward: higher for being close to center, lower for being far

            float reward = (1f - normalized) * CenterLineReward;

            AddReward(reward);



            if (EnableDebugLogging && m_KartStepCount % (LogFrequency * 4) == 0)

            {

                Debug.Log($"[CENTER LINE] Agent: {gameObject.name} | DistanceFromCenter: {distanceFromCenter:F2} | Normalized: {normalized:F3} | Reward: {reward:F4}");

            }

        }



        // Helper method to get distance to track edge

        float GetDistanceToTrackEdge(Vector3 direction)

        {

            Vector3 worldDirection = transform.TransformDirection(direction);

            if (Physics.Raycast(transform.position, worldDirection, out RaycastHit hit, TrackWidth, Mask))

            {

                if (ShowRaycasts)

                {

                    Debug.DrawRay(transform.position, worldDirection * hit.distance, Color.yellow);

                }

                return hit.distance;

            }

            return TrackWidth / 2f; // Default if no hit

        }



        // NEW: Smooth steering reward

        void AddSmoothSteeringReward()

        {

            float steeringChange = Mathf.Abs(m_Steering - m_LastSteering);

           

            // Penalize large steering changes

            if (steeringChange > 0.5f)

            {

                float penalty = steeringChange * SmoothSteeringPenalty;

                AddReward(penalty);

               

                if (EnableDebugLogging && m_KartStepCount % (LogFrequency * 3) == 0)

                {

                    Debug.Log($"[SMOOTH STEERING] Agent: {gameObject.name} | Change: {steeringChange:F3} | " +

                             $"Penalty: {penalty:F4}");

                }

            }

           

            m_LastSteering = m_Steering;

        }



        // NEW: Optimal speed reward

        void AddOptimalSpeedReward()

        {

            float currentSpeed = m_Kart.LocalSpeed();

           

            if (currentSpeed >= MinSpeedThreshold && currentSpeed <= MaxSpeedThreshold)

            {

                // Reward for being in optimal speed range

                float speedReward = OptimalSpeedReward;

                AddReward(speedReward);

            }

            else if (currentSpeed < MinSpeedThreshold)

            {

                // Penalize for going too slow

                AddReward(SlowSpeedPenalty);

            }

            // No penalty for going too fast, just no reward

        }



        // NEW: Progress reward for moving toward next checkpoint

        void AddProgressReward()

        {

            var next = (m_CheckpointIndex + 1) % Colliders.Length;

            var nextCheckpoint = Colliders[next];

            if (nextCheckpoint == null) return;



            float currentDistance = Vector3.Distance(transform.position, nextCheckpoint.transform.position);

           

            // Reward if getting closer to checkpoint

            if (currentDistance < m_LastDistanceToNextCheckpoint)

            {

                float progressMade = m_LastDistanceToNextCheckpoint - currentDistance;

                float reward = progressMade * ProgressReward;

                AddReward(reward);

            }

           

            m_LastDistanceToNextCheckpoint = currentDistance;

        }



        void LateUpdate()

        {

            switch (Mode)

            {

                case AgentMode.Inferencing:

                    if (ShowRaycasts)

                        Debug.DrawRay(transform.position, Vector3.down * GroundCastDistance, Color.cyan);



                    // We want to place the agent back on the track if the agent happens to launch itself outside of the track.

                    if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, GroundCastDistance, TrackMask)

                        && ((1 << hit.collider.gameObject.layer) & OutOfBoundsMask) > 0)

                    {

                        if (EnableDebugLogging)

                        {

                            Debug.Log($"[OUT OF BOUNDS] Agent {gameObject.name} fell off track at {transform.position}. Resetting to checkpoint {m_CheckpointIndex}");

                        }

                       

                        // Reset the agent back to its last known agent checkpoint

                        var checkpoint = Colliders[m_CheckpointIndex].transform;

                        transform.localRotation = checkpoint.rotation;

                        transform.position = checkpoint.position;

                        m_Kart.Rigidbody.velocity = default;

                        m_Steering = 0f;

                        m_Acceleration = m_Brake = false;

                    }

                    break;

            }

        }



        void OnTriggerEnter(Collider other)

{

    var maskedValue = 1 << other.gameObject.layer;

    var triggered = maskedValue & CheckpointMask;



    FindCheckpointIndex(other, out var index);



    // Only increment if the checkpoint is the expected next one (in order)

    if (triggered > 0 && (index == (m_CheckpointIndex + 1) % Colliders.Length))

    {

        AddReward(PassCheckpointReward);

        m_CheckpointsPassed++;

        m_CheckpointsPassedThisLap++;



        // Reset distance tracking for new checkpoint

        var next = (index + 1) % Colliders.Length;

        var nextCheckpoint = Colliders[next];

        if (nextCheckpoint != null)

        {

            m_LastDistanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.transform.position);

        }



        // Lap completion: agent passed the last checkpoint and now back to first

        if (index == 0 && m_CheckpointIndex == Colliders.Length - 1)

        {

            m_LapsCompleted++;

            m_LastLapTime = Time.time - m_LapStartTime;

            m_LapStartTime = Time.time;



            // Optionally, give a big reward for lap completion

            AddReward(LapCompletionReward);



            if (EnableDebugLogging)

            {

                Debug.Log($"[LAP COMPLETED] Agent: {gameObject.name} | Lap: {m_LapsCompleted} | Lap Time: {m_LastLapTime:F2}s | Total Reward: {GetCumulativeReward():F3}");

            }



            // DO NOT end episode here! Let the agent continue looping.

            m_CheckpointsPassedThisLap = 0; // Reset for new lap

        }



        if (EnableDebugLogging)

        {

            Debug.Log($"[CHECKPOINT PASSED] Agent: {gameObject.name} | Checkpoint: {index} | This Lap: {m_CheckpointsPassedThisLap} | Total: {m_CheckpointsPassed} | Speed: {m_Kart.LocalSpeed():F2} | Cumulative Reward: {GetCumulativeReward():F3}");

        }



        m_CheckpointIndex = index;

    }

    else if (triggered > 0 && EnableDebugLogging)

    {

        Debug.Log($"[CHECKPOINT SKIPPED] Agent: {gameObject.name} | Hit checkpoint {index} but expected {(m_CheckpointIndex + 1) % Colliders.Length} | No reward given");

    }

}



        void OnCollisionEnter(Collision collision)

        {

            foreach (var contact in collision.contacts)

            {

                // If the normal is not mostly up, it's likely a wall

                if (Mathf.Abs(Vector3.Dot(contact.normal, Vector3.up)) < 0.5f)

                {

                    m_WallHitCount++;

                    AddReward(HitPenalty);

                   

                    // Reset to last checkpoint instead of ending episode

                    var checkpoint = Colliders[m_CheckpointIndex].transform;

                    transform.position = checkpoint.position;

                    transform.rotation = checkpoint.rotation;

                    m_Kart.Rigidbody.velocity = Vector3.zero;

                    m_Kart.Rigidbody.angularVelocity = Vector3.zero;

                   

                    if (EnableDebugLogging)

                    {

                        Debug.Log($"[WALL COLLISION] Agent: {gameObject.name} | Wall Hits: {m_WallHitCount} | " +

                                 $"Penalty: {HitPenalty} | Reset to checkpoint {m_CheckpointIndex}");

                    }

                    break;

                }

            }

        }



        void FindCheckpointIndex(Collider checkPoint, out int index)

        {

            for (int i = 0; i < Colliders.Length; i++)

            {

                if (Colliders[i].GetInstanceID() == checkPoint.GetInstanceID())

                {

                    index = i;

                    return;

                }

            }

            index = -1;

        }



        float Sign(float value)

        {

            if (value > 0)

            {

                return 1;

            }

            if (value < 0)

            {

                return -1;

            }

            return 0;

        }



        public override void CollectObservations(VectorSensor sensor)

        {

            m_KartStepCount++;

           

            sensor.AddObservation(m_Kart.LocalSpeed());



            // Add an observation for direction of the agent to the next checkpoint.

            var next = (m_CheckpointIndex + 1) % Colliders.Length;

            var nextCollider = Colliders[next];

            if (nextCollider == null)

                return;



            var direction = (nextCollider.transform.position - m_Kart.transform.position).normalized;

            var velocityDot = Vector3.Dot(m_Kart.Rigidbody.velocity.normalized, direction);

            sensor.AddObservation(velocityDot);



            if (ShowRaycasts)

                Debug.DrawLine(AgentSensorTransform.position, nextCollider.transform.position, Color.magenta);



            m_LastAccumulatedReward = 0.0f;

            m_EndEpisode = false;

            int wallHitsThisStep = 0;

           

            for (var i = 0; i < Sensors.Length; i++)

            {

                var current = Sensors[i];

                var xform = current.Transform;

                var hit = Physics.Raycast(AgentSensorTransform.position, xform.forward, out var hitInfo,

                    current.RayDistance, Mask, QueryTriggerInteraction.Ignore);



                if (ShowRaycasts)

                {

                    Debug.DrawRay(AgentSensorTransform.position, xform.forward * current.RayDistance, Color.green);

                    Debug.DrawRay(AgentSensorTransform.position, xform.forward * current.HitValidationDistance,

                        Color.red);



                    if (hit && hitInfo.distance < current.HitValidationDistance)

                    {

                        Debug.DrawRay(hitInfo.point, Vector3.up * 3.0f, Color.blue);

                    }

                }



                if (hit)

                {

                    if (hitInfo.distance < current.HitValidationDistance)

                    {

                        m_LastAccumulatedReward += HitPenalty;

                        //m_EndEpisode = true;

                        wallHitsThisStep++;

                        m_WallHitCount++;

                       

                        if (EnableDebugLogging)

                        {

                            Debug.Log($"[WALL HIT] Agent: {gameObject.name} | Sensor {i} | Distance: {hitInfo.distance:F2} | " +

                                     $"Hit Object: {hitInfo.collider.name} | Penalty: {HitPenalty} | " +

                                     $"Position: {transform.position} | Speed: {m_Kart.LocalSpeed():F2}");

                        }

                    }

                }



                sensor.AddObservation(hit ? hitInfo.distance : current.RayDistance);

            }



            sensor.AddObservation(m_Acceleration);



            // Debug logging for periodic updates

            if (EnableDebugLogging && m_KartStepCount % LogFrequency == 0)

            {

                Debug.Log($"[STEP {m_KartStepCount}] Agent: {gameObject.name} | Speed: {m_Kart.LocalSpeed():F2} | " +

                         $"Next Checkpoint: {next} | Direction Alignment: {velocityDot:F3} | " +

                         $"Cumulative Reward: {GetCumulativeReward():F3} | Wall Hits: {m_WallHitCount} | Laps: {m_LapsCompleted}");

            }

        }



        public override void OnActionReceived(ActionBuffers actions)

        {

            base.OnActionReceived(actions);

            InterpretDiscreteActions(actions);



            // Find the next checkpoint when registering the current checkpoint that the agent has passed.

            var next = (m_CheckpointIndex + 1) % Colliders.Length;

            var nextCollider = Colliders[next];

            var direction = (nextCollider.transform.position - m_Kart.transform.position).normalized;

            var directionReward = Vector3.Dot(m_Kart.Rigidbody.velocity.normalized, direction);



            if (ShowRaycasts) Debug.DrawRay(AgentSensorTransform.position, m_Kart.Rigidbody.velocity, Color.blue);



            // Calculate and apply original rewards

            float towardsReward = directionReward * TowardsCheckpointReward;

            float accelReward = (m_Acceleration && !m_Brake ? 1.0f : 0.0f) * AccelerationReward;

            float speedRewardValue = m_Kart.LocalSpeed() * SpeedReward;



            AddReward(towardsReward);

            AddReward(accelReward);

            AddReward(speedRewardValue);



            // NEW: Apply additional reward logics

            AddCenterLineReward();

            AddSmoothSteeringReward();

            AddOptimalSpeedReward();

            AddProgressReward();



            // Debug reward breakdown (less frequent to avoid spam)

            if (EnableDebugLogging && m_KartStepCount % (LogFrequency * 2) == 0)

            {

                Debug.Log($"[REWARDS] Agent: {gameObject.name} | Towards: {towardsReward:F4} | " +

                         $"Accel: {accelReward:F4} | Speed: {speedRewardValue:F4} | " +

                         $"Actions: Steering={actions.DiscreteActions[0]}, Throttle={actions.DiscreteActions[1]}");

            }

        }



        public override void OnEpisodeBegin()

{

    float time_horizon = Academy.Instance.EnvironmentParameters.GetWithDefault("time_horizon", 3000.0f);



        // Now, use this value. A common use is to set the Agent's Max Step count.

        this.MaxStep = (int)time_horizon;



        Debug.Log($"Episode starting. Max steps set to: {this.MaxStep}");

       

    m_KartStepCount = 0;

    m_WallHitCount = 0;

    m_CheckpointsPassed = 0;

    m_CheckpointsPassedThisLap = 0;

    m_LapsCompleted = 0;

    m_TotalReward = 0f;

    m_EpisodeStartTime = Time.time;

    m_LapStartTime = Time.time;

    m_LastLapTime = 0f;

    m_WrongDirectionTimer = 0f;

    m_LastSteering = 0f;

    m_LastDistanceToNextCheckpoint = float.MaxValue;



    if (EnableCSVLogging)

    {

        m_TotalDistanceTraveled = 0f;

        m_MaxSpeedThisEpisode = 0f;

        m_MinSpeedThisEpisode = float.MaxValue;

        m_SpeedSum = 0f;

        m_SpeedSamples = 0;

        m_LastPosition = Vector3.zero;

    }



    // Always start at checkpoint 0 for consistency

    m_CheckpointIndex = 0;

    var collider = Colliders[m_CheckpointIndex];

    transform.localRotation = collider.transform.rotation;

    transform.position = collider.transform.position;

    m_Kart.Rigidbody.velocity = Vector3.zero;

    m_Kart.Rigidbody.angularVelocity = Vector3.zero;

    m_Acceleration = false;

    m_Brake = false;

    m_Steering = 0f;



    var next = (m_CheckpointIndex + 1) % Colliders.Length;

    var nextCheckpoint = Colliders[next];

    if (nextCheckpoint != null)

    {

        m_LastDistanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.transform.position);

    }



    if (EnableDebugLogging)

    {

        Debug.Log($"[EPISODE START] Agent: {gameObject.name} | Starting at checkpoint: {m_CheckpointIndex} | Position: {transform.position} | Mode: {Mode}");

    }

}



        void InterpretDiscreteActions(ActionBuffers actions)

        {

            m_TargetSteering = actions.DiscreteActions[0] - 1f;

            m_Acceleration = actions.DiscreteActions[1] >= 1.0f;

            m_Brake = actions.DiscreteActions[1] < 1.0f;

        }



        public InputData GenerateInput()

        {

            return new InputData

            {

                Accelerate = m_Acceleration,

                Brake = m_Brake,

                TurnInput = m_Steering

            };

        }



        // Additional debug methods

        public void LogCurrentState()

        {

            if (EnableDebugLogging)

            {

                Debug.Log($"[STATE] Agent: {gameObject.name} | Checkpoint: {m_CheckpointIndex}/{Colliders.Length} | " +

                         $"Speed: {m_Kart.LocalSpeed():F2} | Position: {transform.position} | " +

                         $"Steering: {m_Steering:F2} | Accel: {m_Acceleration} | Brake: {m_Brake} | Laps: {m_LapsCompleted}");

            }

        }



        // Call this from inspector or other scripts to toggle debug mode

        [ContextMenu("Toggle Debug Logging")]

        public void ToggleDebugLogging()

        {

            EnableDebugLogging = !EnableDebugLogging;

            Debug.Log($"Debug logging {(EnableDebugLogging ? "enabled" : "disabled")} for agent {gameObject.name}");

        }



        [ContextMenu("Toggle CSV Logging")]

        public void ToggleCSVLogging()

        {

            EnableCSVLogging = !EnableCSVLogging;

            Debug.Log($"CSV logging {(EnableCSVLogging ? "enabled" : "disabled")} for agent {gameObject.name}");

        }

    }

}