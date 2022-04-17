using Pogo;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class KillTrigger : MonoBehaviour
{
    public UnityEvent OnTriggered;
    public KillType Type;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggered.Invoke();
        PogoGameManager.PogoInstance.KillPlayer(Type);
    }
}