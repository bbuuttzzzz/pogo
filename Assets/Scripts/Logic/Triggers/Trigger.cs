using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Trigger : MonoBehaviour
{
    public UnityEvent OnActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (CanTrigger(other))
        {
            OnActivated.Invoke();
        }
    }

    public virtual bool CanTrigger(Collider other)
    {
        return true;
    }
}