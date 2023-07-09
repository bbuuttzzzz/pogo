using Pogo;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class KillTrigger : MonoBehaviour
{
    public UnityEvent OnTriggered;
    public KillTypeDescriptor Type;

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG
        if (Type == null)
        {
            throw new MissingReferenceException(nameof(Type));
        }
#endif

        OnTriggered.Invoke();

        Vector3 origin = GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center);
        Vector3 normal = (other.bounds.center - origin).normalized;

        PogoGameManager.PogoInstance.KillPlayer(new PlayerDeathData(Type, transform, origin, normal));
    }
}