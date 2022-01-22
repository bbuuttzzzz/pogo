using UnityEngine;

class CameraWallAntiClipper : MonoBehaviour
{
    Vector3 desiredLocalPosition;

    Transform parent;

    public float WallBufferDistance = 0.1f;

    private void Awake()
    {
        parent = transform.parent;
        desiredLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        Vector3 start = parent.transform.position;
        Vector3 desired = parent.TransformPoint(desiredLocalPosition);
        float maxDistance = (desired - start).magnitude + WallBufferDistance;
        bool didHit = Physics.Raycast(ray: new Ray(start, desired - start),
            maxDistance: maxDistance,
            hitInfo: out RaycastHit hitinfo,
            layerMask: LAYERMASK.SOLIDS_ONLY);

        if (didHit)
        {
            Vector3 finalPosition = start + (desired - start) * (hitinfo.distance - WallBufferDistance) / maxDistance;
            transform.position = finalPosition;
        }
        else
        {
            transform.position = desired;
        }
    }
}
