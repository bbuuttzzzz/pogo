using UnityEngine;

public class LinearMover : MonoBehaviour
{
    public Transform target;

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
        initialPosition = target.transform.position;
        targetPosition = target.transform.position;
        arrived = true;
        GoToWaypoint(InitialWaypoint);
        ArriveNow();
    }

    private void Update()
    {
        if (!arrived)
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, targetPosition, TravelSpeed * Time.deltaTime);
            if (target.transform.position == targetPosition)
            {
                arrived = true;
            }
        }
    }

    public void ArriveNow()
    {
        target.transform.position = targetPosition;
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
            Gizmos.DrawIcon(transform.TransformPoint(localPosition), "sp_flag.tiff", false, Color.blue);
        }
    }
}