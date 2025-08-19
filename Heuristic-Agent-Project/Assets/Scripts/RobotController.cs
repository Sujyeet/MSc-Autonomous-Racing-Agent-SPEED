using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class RobotController : MonoBehaviour {
    // naming constraints do not change
    [SerializeField] private WheelCollider FLC;
    [SerializeField] private WheelCollider FRC;
    [SerializeField] private WheelCollider RLC;
    [SerializeField] private WheelCollider RRC;

    [SerializeField] private Transform FLT;
    [SerializeField] private Transform FRT;
    [SerializeField] private Transform RLT;
    [SerializeField] private Transform RRT;

    [SerializeField] private Transform FRS;
    [SerializeField] private Transform L1S;
    [SerializeField] private Transform L2S;
    [SerializeField] private Transform L3S;
    [SerializeField] private Transform R1S;
    [SerializeField] private Transform R2S;
    [SerializeField] private Transform R3S;
    [SerializeField] private Transform ORS;

    private Rigidbody rb;
    private float siAngle;
    private float Robotspeed = 1f;
    private int time = 0;
    private float difference;

    private LayerMask road;
    private LayerMask obstacle;
    private LayerMask landscape;

    private float robot = 0;
    [SerializeField] private float AngleTarget = 0f;
    private bool steep;

    private bool FrontSensorActive;
    private bool PrevFrontSensorSensorActive;
    private Vector3 FrontSensorPosition;
    private RaycastHit FrontSensorHit;
    private float FrontSensorNoSideFound;

    private bool SensorL1SensorActive;
    private bool PreviousSL1SensorActive;
    private Vector3 SensorL1Position;
    private RaycastHit SensorL1Hit;
    private float SensorL1NoSideFound;

    private bool SensorR1SensorActive;
    private bool PreviousSensorR1SensorActive;
    private Vector3 SensorR1Position;
    private RaycastHit SensorR1Hit;
    private float SensorR1NoSideFound;

    private bool SensorL2SensorActive;
    private bool PreviousSensorL2SensorActive;
    private Vector3 SensorL2Position;
    private RaycastHit SensorL2Hit;
    private float SensorL2NoSideFound;

    private bool SensorR2SensorActive;
    private bool PreviousSensorR2SensorActive;
    private Vector3 SensorR2Position;
    private RaycastHit SensorR2Hit;
    private float SensorR2NoSideFound;

    private bool SensorL3SensorActive;
    private bool PreviousSensorL3SensorActive;
    private Vector3 SensorL3Position;
    private RaycastHit SensorL3Hit;
    private float SensorL3NoSideFound;

    private bool SensorR3SensorActive;
    private bool PreviousSensorR3SensorActive;
    private Vector3 SensorR3Position;
    private RaycastHit SensorR3Hit;
    private float SensorR3NoSideFound;

    private Vector3 MergedSensor1Position;
    private Vector3 MergedSensor2Position;
    private Vector3 MergedSensor3Position;

    Transform FLS_transform;

    private void Start() {
        GameObject FLS = Instantiate(FRS.gameObject, FRS.parent);
        FLS.name = "FLS";
        FLS.transform.rotation = FRS.transform.rotation;
        FLS.transform.position = new Vector3(FRS.transform.position.x, FRS.transform.position.y, FRS.transform.position.z - 6);
        FLS_transform = FLS.transform;
        FLS.GetComponent<MeshRenderer>().enabled = false;

        //GameObject L1S_ = Instantiate(L1S.gameObject, L1S);
        //L1S_t = L1S_.transform;
        //L1S_t.position = new Vector3(L1S_t.position.x-0.33f, L1S_t.position.y, L1S_t.position.z);
        //GameObject R1S_ = Instantiate(R1S.gameObject, R1S);
        //R1S_t = R1S_.transform;
        //R1S_t.position = new Vector3(R1S_t.position.x + 0.33f, R1S_t.position.y, R1S_t.position.z);
        //---------------------------
        rb = GetComponent<Rigidbody>();
        rb.mass = 1000;
        road = 1 << LayerMask.NameToLayer("Road");
        obstacle = 1 << LayerMask.NameToLayer("Obs");
        landscape = 1 << LayerMask.NameToLayer("LS");


        FrontSensorActive = true;
        PrevFrontSensorSensorActive = true;
        SensorL1SensorActive = true;
        PreviousSL1SensorActive = true;
        SensorR1SensorActive = true;
        PreviousSensorR1SensorActive = true;
        SensorL2SensorActive = true;
        PreviousSensorL2SensorActive = true;
        SensorR2SensorActive = true;
        PreviousSensorR2SensorActive = true;
        SensorL3SensorActive = true;
        PreviousSensorL3SensorActive = true;
        SensorR3SensorActive = true;
        PreviousSensorR3SensorActive = true;

        //StartCoroutine(ListRoadPositions());
        //StartCoroutine(CountTime());
    }


    private void Update() {
        FLC.steerAngle = Mathf.MoveTowards(FLC.steerAngle, AngleTarget, 20f * Time.deltaTime);
        FRC.steerAngle = Mathf.MoveTowards(FRC.steerAngle, AngleTarget, 20f * Time.deltaTime);
    }

    bool stop = false;
    private void FixedUpdate() {

        if (!stop) {
            CastDownRay(FLS_transform, stop);
        }

        if (stop) {
            return;
        }

        robot = (Mathf.PingPong(Time.time * 5f, 1.5f) / 2f) - 0.5f;
        //Debug.Log("Robot :: " + robot);
        RaycastFromSensorSFR();
        RaycastFromSensorSL1();
        RaycastFromSensorSR1();
        RaycastFromSensorSL2();
        RaycastFromSensorSR2();
        RaycastFromSensorSL3();
        RaycastFromSensorSR3();

        MergedSensor1Position = SensorL1Position + (-SensorL1NoSideFound + 0.5f + SensorR1NoSideFound) * (SensorR1Position - SensorL1Position);
        MergedSensor2Position = SensorL2Position + (-SensorL2NoSideFound + 0.5f + SensorR2NoSideFound) * (SensorR2Position - SensorL2Position);
        MergedSensor3Position = SensorL3Position + (-SensorL3NoSideFound + 0.5f + SensorR3NoSideFound) * (SensorR3Position - SensorL3Position);

        if (ORS.transform.eulerAngles.x > 180f) {
            difference = 360f - ORS.transform.eulerAngles.x;
        } else {
            difference = ORS.transform.eulerAngles.x;
        }

        if (new Ray(FRS.position, FRS.TransformDirection(new Vector3(0f, 0f, 1f))).direction.y > 0) {
            steep = true;
        } else {
            steep = false;
        }

        siAngle = Vector3.SignedAngle(transform.forward, ((MergedSensor1Position + MergedSensor2Position) / 2f) - transform.position, transform.up);

        if (siAngle > 12f || siAngle < -12f) {
            if (siAngle > 14f) {
                siAngle = 12f;
            }

            if (siAngle < -14f) {
                siAngle = -12f;
            }

            AngleTarget = siAngle;
        } else {
            AngleTarget = 0f;
        }

        //Debug.Log("Angle Target " + AngleTarget);

        FLC.brakeTorque = 0f;
        FRC.brakeTorque = 0f;

        Movement();

        //Debug.Log("SPEED: " + rigidBody.velocity.magnitude);

        FLC.motorTorque = 50f * Robotspeed;
        FRC.motorTorque = 50f * Robotspeed;

        DebugVectors();

        Vector3 pos;
        Quaternion quat;

        FLC.GetWorldPose(out pos, out quat);
        FLT.transform.rotation = quat;

        FRC.GetWorldPose(out pos, out quat);
        FRT.transform.rotation = quat;

        RLT.transform.localRotation = Quaternion.Euler(FLT.transform.localRotation.eulerAngles.x, RLT.transform.localRotation.eulerAngles.y, RLT.transform.localRotation.eulerAngles.z);
        RRT.transform.localRotation = Quaternion.Euler(FRT.transform.localRotation.eulerAngles.x, RRT.transform.localRotation.eulerAngles.y, RRT.transform.localRotation.eulerAngles.z);

        if (Vector3.Distance(transform.position, new Vector3(4.62f, 21.63f, -196.2006f)) < 10f) {
            FLC.brakeTorque = 1000f;
            FRC.brakeTorque = 1000f;
        }
    }

    private void CastDownRay(Transform sensor, bool stop) {
        float rayRange = 30f; // Range of the ray

        Vector3 origin = sensor.position;
        Vector3 direction = Vector3.down;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * rayRange, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, rayRange)) {
            //Debug.Log("Road detected by RoadEndSensor at: " + hit.point);
        } else {
            //Debug.Log("No road detected - possible road end.");
            Stop();
        }
    }

    private bool ShootRay(Transform sensorTransform, float XVector, float ZVector, Color color, ref RaycastHit hit) {
        //Debug.DrawRay(sensorTransform.position, sensorTransform.TransformDirection(new Vector3(XVector, sensorAngle, ZVector)) * 20, color);

        return Physics.Raycast(sensorTransform.position, sensorTransform.TransformDirection(new Vector3(XVector, robot, ZVector)), out hit, 13f, road);
    }

    private void SetSensor(Transform sensorTransform, bool sensorActive, bool previousSensorActive, ref Vector3 sensorPosition, ref RaycastHit hit, ref float noSideFound) {
        if (sensorActive != previousSensorActive && sensorActive == true) {
            sensorPosition = hit.point;
            float distance = Vector3.Distance(sensorTransform.position, hit.point);

            if (distance > 10f) {
                noSideFound = 0f;
            } else {
                noSideFound = 0f;
            }
        }
    }

    //Transform L1S_t;
    //Transform R1S_t;
    private void RaycastFromSensorSFR() {
        // Front Center Raycast
        Debug.DrawRay(FRS.position, FRS.TransformDirection(Vector3.forward) * 20f, Color.white);
        if (Physics.Raycast(FRS.position, FRS.TransformDirection(Vector3.forward), out FrontSensorHit, 20f, obstacle)) {
            //Debug.Log($"Front Obstacle detected at {FrontSensorHit.distance}");
            HandleObstacleAvoidance("center", FrontSensorHit.distance);
        }

        // Left Raycast from L1S (slightly rotated outward)
        Debug.DrawRay(L1S.position, L1S.TransformDirection(new Vector3(-0.12f, 0f, 1f).normalized) * 20f, Color.green);
        if (Physics.Raycast(L1S.position, L1S.TransformDirection(new Vector3(-0.12f, 0f, 1f).normalized), out RaycastHit leftHit, 20f, obstacle)) {
            //Debug.Log($"Front-Left Obstacle detected at {leftHit.distance}");
            HandleObstacleAvoidance("left", leftHit.distance);
        }

        // Right Raycast from R1S (slightly rotated outward)
        Debug.DrawRay(R1S.position, R1S.TransformDirection(new Vector3(0.12f, 0f, 1f).normalized) * 20f, Color.green);
        if (Physics.Raycast(R1S.position, R1S.TransformDirection(new Vector3(0.12f, 0f, 1f).normalized), out RaycastHit rightHit, 20f, obstacle)) {
            //Debug.Log($"Front-Right Obstacle detected at {rightHit.distance}");
            HandleObstacleAvoidance("right", rightHit.distance);
        }
    }

    private void HandleObstacleAvoidance(string direction, float distance) {
        Debug.Log("Distance :: " + distance);
        if (distance < 20f) { // Adjust threshold as needed
            Debug.Log($"Obstacle too close on the {direction}. Avoiding...");
            switch (direction) {
                case "center":
                    // If the obstacle is directly ahead, slow down and consider steering to either side
                    FLC.steerAngle = 7f; // Default to steering right
                    FRC.steerAngle = 7f;
                    FLC.brakeTorque = 250f;
                    FRC.brakeTorque = 250f;
                    break;

                case "left":
                    // If the obstacle is to the left, steer to the right
                    FLC.steerAngle = 7f;
                    FRC.steerAngle = 7f;
                    FLC.brakeTorque = 250f;
                    FRC.brakeTorque = 250f;
                    break;

                case "right":
                    // If the obstacle is to the right, steer to the left
                    FLC.steerAngle = -7f;
                    FRC.steerAngle = -7f;
                    FLC.brakeTorque = 250f;
                    FRC.brakeTorque = 250f;
                    break;
            }
        }
    }


    private void RaycastFromSensorSL1() {
        SensorL1SensorActive = ShootRay(L1S, -0.51449574614f, 0.85749291024f, Color.green, ref SensorL1Hit);
        SetSensor(L1S, SensorL1SensorActive, PreviousSL1SensorActive, ref SensorL1Position, ref SensorL1Hit, ref SensorL1NoSideFound);

        PreviousSL1SensorActive = SensorL1SensorActive;
    }

    private void RaycastFromSensorSR1() {
        SensorR1SensorActive = ShootRay(R1S, 0.51449574614f, 0.85749291024f, Color.green, ref SensorR1Hit);
        SetSensor(R1S, SensorR1SensorActive, PreviousSensorR1SensorActive, ref SensorR1Position, ref SensorR1Hit, ref SensorR1NoSideFound);

        PreviousSensorR1SensorActive = SensorR1SensorActive;
    }

    private void RaycastFromSensorSL2() {
        SensorL2SensorActive = ShootRay(L2S, -0.894427182f, 0.447213591f, Color.magenta, ref SensorL2Hit);
        SetSensor(L2S, SensorL2SensorActive, PreviousSensorL2SensorActive, ref SensorL2Position, ref SensorL2Hit, ref SensorL2NoSideFound);

        PreviousSensorL2SensorActive = SensorL2SensorActive;
    }

    private void RaycastFromSensorSR2() {
        SensorR2SensorActive = ShootRay(R2S, 0.894427182f, 0.447213591f, Color.magenta, ref SensorR2Hit);
        SetSensor(R2S, SensorR2SensorActive, PreviousSensorR2SensorActive, ref SensorR2Position, ref SensorR2Hit, ref SensorR2NoSideFound);

        PreviousSensorR2SensorActive = SensorR2SensorActive;
    }

    private void RaycastFromSensorSL3() {
        SensorL3SensorActive = ShootRay(L3S, -1f, 0f, Color.blue, ref SensorL3Hit);
        SetSensor(L3S, SensorL3SensorActive, PreviousSensorL3SensorActive, ref SensorL3Position, ref SensorL3Hit, ref SensorL3NoSideFound);

        PreviousSensorL3SensorActive = SensorL3SensorActive;
    }

    private void RaycastFromSensorSR3() {
        SensorR3SensorActive = ShootRay(R3S, 1f, 0f, Color.blue, ref SensorR3Hit);
        SetSensor(R3S, SensorR3SensorActive, PreviousSensorR3SensorActive, ref SensorR3Position, ref SensorR3Hit, ref SensorR3NoSideFound);

        PreviousSensorR3SensorActive = SensorR3SensorActive;
    }

    private void Movement() {
        if (rb.velocity.magnitude < 4f) {
            if (rb.velocity.magnitude < 2f) {
                if (steep && difference > 10f) {
                    Robotspeed = 20f;
                } else {
                    Robotspeed = 10f;
                }
            } else {
                if (steep && difference > 10f) {
                    Robotspeed = 20f;
                } else {
                    Robotspeed = 0.7f;
                }
            }
        } else {
            Robotspeed = 0.7f;
        }
        Debug.Log(Robotspeed);

        if (rb.velocity.magnitude > 6f) {
            FLC.brakeTorque = 200f;
            FRC.brakeTorque = 200f;
        }

        if (rb.velocity.magnitude > 8f) {
            FLC.brakeTorque = 500f;
            FRC.brakeTorque = 500f;
        }
    }

    private void Stop() {
        FLC.brakeTorque = 5000f;
        FRC.brakeTorque = 5000f;
        stop = true;
    }
    private void DebugVectors() {
        //Debug.DrawRay(MergedS3Position, MergedS2Position - MergedS3Position, Color.yellow, 0.1f);
        //Debug.DrawRay(MergedS2Position, MergedS1Position - MergedS2Position, Color.yellow, 0.1f);
        //Debug.DrawRay(MergedS3Position, ((MergedS1Position + MergedS2Position) / 2f) - MergedS3Position, Color.cyan, 0.1f);
        //Debug.DrawRay(transform.position, ((MergedS1Position + MergedS2Position) / 2f) - transform.position, Color.cyan, 0.1f);
    }

    IEnumerator RoadPositions() {
        yield return new WaitForSeconds(1f);

        //Debug.Log("SFR: " + SFRPosition + " SL1: " + SL1Position + " SR1: " + SR1Position);
        //Debug.Log(" SL2: " + SL2Position + " SR2: " + SR2Position);
        //Debug.Log(" SL3: " + SL3Position + " SR3: " + SR3Position);

        //Debug.Log("MergedS1Position: " + MergedS1Position);
        //Debug.Log("MergedS2Position: " + MergedS2Position);
        //Debug.Log("MergedS3Position: " + MergedS3Position);

        StartCoroutine(RoadPositions());
    }

    IEnumerator CTime() {
        yield return new WaitForSeconds(10f);

        time += 10;

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        Debug.Log(timeSpan.ToString());

        StartCoroutine(CTime());
    }
}
