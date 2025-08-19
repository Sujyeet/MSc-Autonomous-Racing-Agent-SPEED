using UnityEngine;

public class WaypointKartAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 10f;
    public float turnSpeed = 5f;
    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 target = waypoints[currentIndex].position;
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        // Rotate toward waypoint
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);

        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Next waypoint?
        if (dir.magnitude < 3f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }
}
