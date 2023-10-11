using Logic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnterExitTrigger : MonoBehaviour
{
    public TriggedByColliderEvent OnEnter;
    public TriggedByColliderEvent OnExit;

    bool triggeredThisFrame = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!triggeredThisFrame && CanTrigger(other))
        {
            OnEnter.Invoke(other);
            triggeredThisFrame = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!triggeredThisFrame && CanTrigger(other))
        {
            OnExit.Invoke(other);
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