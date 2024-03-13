using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
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

    public void FixTriggerSettings()
    {
        gameObject.layer = LAYER.trigger;
    }
}