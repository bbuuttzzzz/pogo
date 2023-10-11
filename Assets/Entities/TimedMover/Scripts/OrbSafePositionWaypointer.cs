using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardPhysics;
using WizardUtils;

[RequireComponent(typeof(OrbSafeMover))]
public class OrbSafePositionWaypointer : PhysicsTimeWaypointer<Vector3>
{
    Vector3 origin;
    private OrbSafeMover mover;

    public override void Awake()
    {
        mover = GetComponent<OrbSafeMover>();
        origin = transform.position;
        base.Awake();
    }

    private void OnDrawGizmosSelected()
    {
        if (Waypoints == null) return;
        if (Application.isPlaying)
        {

            foreach (Vector3 worldOffset in Waypoints)
            {
                Gizmos.DrawIcon(origin + worldOffset, "sp_flag.tiff", false, Color.blue);
            }
        }
        else
        {
            foreach (Vector3 worldOffset in Waypoints)
            {
                Gizmos.DrawIcon(transform.position + worldOffset, "sp_flag.tiff", false, Color.blue);
            }
        }
    }

    protected override Vector3 GetCurrentPhysicsValue()
    {
        return transform.position - origin;
    }

    protected override void InterpolateAndApplyPhysics(Vector3 startValue, Vector3 endValue, float i)
    {
        Vector3 newPositionFromOrigin = Vector3.Lerp(startValue, endValue, i);

        mover.PhysicsMoveTo(origin + newPositionFromOrigin);
    }

    protected override void InterpolateAndApplyRender(Vector3 startValue, Vector3 endValue, float i)
    {
        Vector3 newPositionFromOrigin = Vector3.Lerp(startValue, endValue, i);

        mover.RendererMoveTo(origin + newPositionFromOrigin);
    }
}
