using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorWaypointer : MonoBehaviour
{
    public Renderer renderer;
    Color initialColor;
    Color lastColor;
    Color nextColor;
    bool arrived;
    float changeStartTime;

    [Tooltip("how long the transition will take in seconds")]
    public float TransitionDuration = 10;

    /// <summary>
    /// Colors to change to. current color is already stored as waypoint -1
    /// </summary>
    public Color[] Waypoints;

    /// <summary>
    /// What waypoint to start at. -1 is stay in its original position
    /// </summary>
    public int InitialWaypoint = -1;

    private void Awake()
    {
        initialColor = renderer.material.color;
        arrived = true;
        GoToWaypoint(InitialWaypoint);
        FinishColorChangeNow();
    }

    private void Update()
    {
        if (!arrived)
        {
            renderer.material.color = Color.Lerp(lastColor, nextColor, (Time.time - changeStartTime) / TransitionDuration);
            if (Time.time > changeStartTime + TransitionDuration)
            {
                arrived = true;
            }
        }
    }

    public void SnapToWaypoint(int index)
    {
        GoToWaypoint(index);
        FinishColorChangeNow();
    }

    public void FinishColorChangeNow()
    {
        renderer.material.color = nextColor;
        arrived = true;
    }

    public void GoToWaypoint(int index)
    {
        lastColor = renderer.material.color;
        nextColor = getWaypoint(index);
        changeStartTime = Time.time;
        arrived = false;
    }

    Color getWaypoint(int index)
    {
        if (index < 0 || index >= Waypoints.Length) return initialColor;

        return Waypoints[index];
    }
}
