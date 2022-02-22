using Logic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class StayTrigger : MonoBehaviour
{
    public TriggedByColliderEvent WhileInside;

    bool triggeredThisFrame = false;
    private void OnTriggerStay(Collider other)
    {
        if (!triggeredThisFrame && CanTrigger(other))
        {
            WhileInside.Invoke(other);
            triggeredThisFrame = true;
        }
    }

    private void Update()
    {
        triggeredThisFrame = false;
    }

    public virtual bool CanTrigger(Collider other)
    {
        return true;
    }

}