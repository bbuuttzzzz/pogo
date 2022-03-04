using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Trigger : MonoBehaviour
{
    public UnityEvent OnActivated;

    public bool CanTrigger { get; set; } = true;

    private void OnTriggerEnter(Collider other)
    {
        if (CanTrigger && ColliderCanTrigger(other))
        {
            OnActivated.Invoke();
        }
    }

    public virtual bool ColliderCanTrigger(Collider other)
    {
        return true;
    }
}