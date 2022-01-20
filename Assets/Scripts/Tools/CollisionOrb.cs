using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CollisionOrb : MonoBehaviour
{
    public bool QueryExits;

    new SphereCollider collider;
    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        recalculateLayerMask();
        Disjoint();
    }

    private void Update()
    {
        Move();
    }

    Vector3 lastRecordedPosition;
    void Move()
    {
        bool didHit = CheckPath(lastRecordedPosition, currentCenter, out RaycastHit hitinfo);
        if (didHit)
        {

        }

        if(QueryExits)
        {
            bool didExit = CheckPath(currentCenter, lastRecordedPosition, out RaycastHit exitInfo);
            if (didHit)
            {

            }
        }
    }

    bool CheckPath(Vector3 start, Vector3 finish, out RaycastHit hitInfo)
    {
        Vector3 direction = finish - start;
        float maxDistance = (finish - start).magnitude;
        direction = direction / maxDistance;
        return Physics.SphereCast(
            ray: new Ray(start, direction),
            radius: collider.radius * transform.lossyScale.x,
            maxDistance: maxDistance,
            hitInfo: out hitInfo,
            layerMask: CurrentLayerMask);
    }

    /// <summary>
    /// Reset last recorded position. you should call this immediately after teleporting
    /// </summary>
    public void Disjoint()
    {
        lastRecordedPosition = currentCenter;
    }

    Vector3 currentCenter => transform.TransformPoint(collider.center);

    #region layermask
    int cachedLayer;
    int cachedLayerMask;

    int CurrentLayerMask
    {
        get
        {
            if (cachedLayer != gameObject.layer)
            {
                recalculateLayerMask();
            }

            return cachedLayerMask;
        }
    }

    void recalculateLayerMask()
    {
        cachedLayer = gameObject.layer;
        cachedLayerMask = LAYERMASK.MaskForLayer(cachedLayer);
    }


    #endregion
}
