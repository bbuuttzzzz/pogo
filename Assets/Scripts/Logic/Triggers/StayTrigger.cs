using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class StayTrigger : MonoBehaviour
{
    public UnityEvent WhileInside;

    bool triggeredThisFrame = false;
    private void OnTriggerStay(Collider other)
    {
        if (!triggeredThisFrame && CanTrigger(other))
        {
            WhileInside.Invoke();
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