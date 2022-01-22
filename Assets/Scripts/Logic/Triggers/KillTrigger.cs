using Pogo;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class KillTrigger : MonoBehaviour
{
    public UnityEvent OnTriggered;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggered.Invoke();
        PogoGameManager.KillPlayer();
    }
}