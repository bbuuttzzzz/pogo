using UnityEngine;

public class LinearMover : MonoBehaviour
{
    Vector3 initialPosition;
    Vector3 targetPosition;
    bool arrived;

    /// <summary>
    /// Speed in meters per second for this object to move
    /// </summary>
    public float TravelSpeed = 10;

    /// <summary>
    /// local offsets from initialPosition for this mover to travel to. (0,0,0) is already stored as waypoint -1
    /// </summary>
    public Vector3[] Waypoints;

    /// <summary>
    /// What waypoint to start at. -1 is stay in its original position
    /// </summary>
    public int InitialWaypoint = -1;

    private void Awake()
    {
        initialPosition = transform.position;
        targetPosition = transform.position;
        arrived = true;
        GoToWaypoint(InitialWaypoint);
        ArriveNow();
    }

    private void Update()
    {
        if (!arrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, TravelSpeed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                arrived = true;
            }
        }
    }

    public void ArriveNow()
    {
        transform.position = targetPosition;
        arrived = true;
    }

    public void GoToWaypoint(int index)
    {
        targetPosition = getWaypointWorldPoint(index);
        arrived = false;
    }

    Vector3 getWaypointWorldPoint(int index)
    {
        if (index < 0 || index >= Waypoints.Length) return initialPosition;

        return transform.TransformPoint(Waypoints[index]);
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Vector3 localPosition in Waypoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawIcon(transform.TransformPoint(localPosition), "sp_flag.tiff");
        }
    }
}