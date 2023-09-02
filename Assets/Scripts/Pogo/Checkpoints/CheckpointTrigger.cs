using Pogo;
using Pogo.Checkpoints;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointTrigger : Trigger
{
    public CheckpointDescriptor Descriptor;
    public Transform RespawnPoint;
    public UnityEvent OnEnteredNotActivated;

    public override bool ColliderCanTrigger(Collider other)
    {
        if (WizardUtils.GameManager.GameInstanceIsValid())
        {
            bool success = PogoGameManager.PogoInstance.TryRegisterRespawnPoint(RespawnPoint);
            if (!success) OnEnteredNotActivated?.Invoke();
            return success;
        }
        else
            return false;
    }
}