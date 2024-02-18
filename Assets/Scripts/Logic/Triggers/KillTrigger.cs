using Pogo;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class KillTrigger : MonoBehaviour
{
    public UnityEvent OnTriggered;
    public KillTypeDescriptor Type;
    public bool DoExpensiveOriginStuff;
    public Material DefaultMaterial;

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG
        if (Type == null)
        {
            throw new MissingReferenceException(nameof(Type));
        }
#endif

        OnTriggered.Invoke();

        // try and find a good surface normal and origin

        FindOriginResult result;

        if (!DoExpensiveOriginStuff || !TryFindOriginFrom(other.bounds.center, out result))
        {
            result.origin = GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center);
            result.normal = (other.bounds.center - result.origin).normalized;
        }

        PogoGameManager.PogoInstance.KillPlayer(new PlayerDeathData(Type, transform, result.origin, result.normal));
    }

    private bool TryFindOriginFrom(Vector3 testPoint, out FindOriginResult result)
    {
        Collider myCollider = GetComponent<Collider>();
        Ray ray = new Ray(testPoint, myCollider.bounds.center - testPoint);
        if (myCollider.Raycast(ray, out RaycastHit hitInfo, int.MaxValue))
        {
            result.normal = hitInfo.normal;
            result.origin = hitInfo.point;
            return true;
        }

        result = default;
        return false;
    }

    private struct FindOriginResult
    {
        public Vector3 origin;
        public Vector3 normal;
    }
}