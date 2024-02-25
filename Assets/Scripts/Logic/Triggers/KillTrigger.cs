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

        if (!DoExpensiveOriginStuff || !TryFindOriginFrom(other, out result))
        {
            // this is fallback
            result.origin = GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center);
            result.normal = (other.bounds.center - result.origin).normalized;
        }

        PogoGameManager.PogoInstance.KillPlayer(new PlayerDeathData(Type, transform, result.origin, result.normal));
    }

    private bool TryFindOriginFrom(Collider other, out FindOriginResult result)
    {
        Collider myCollider = GetComponent<Collider>();

        if (Physics.ComputePenetration(
            other, other.transform.position, other.transform.rotation,
            myCollider, myCollider.transform.position, myCollider.transform.rotation,
            out Vector3 direction,
            out float distance))
        {
            result = new FindOriginResult
            {
                normal = direction,
                origin = other.ClosestPointOnBounds(other.bounds.center - 100 * distance * direction)
            };
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