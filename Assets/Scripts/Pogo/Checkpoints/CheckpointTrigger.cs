using Pogo;
using Pogo.Checkpoints;
using System;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointTrigger : Trigger
{
    public CheckpointDescriptor Descriptor;
    public Transform RespawnPoint;
    public UnityEvent OnEnteredNotActivated;

    public UnityEvent OnAheadCheckpointLoaded;

    public void NotifyCheckpointLoad(CheckpointDescriptor loadedCheckpoint)
    {
        if (PlayerPassedThisCheckpoint(loadedCheckpoint))
        {
            OnAheadCheckpointLoaded?.Invoke();
        }
    }

    private bool PlayerPassedThisCheckpoint(CheckpointDescriptor other)
    {
        if (other == Descriptor) return true;

        if (other.Chapter.Number > Descriptor.Chapter.Number)
        {
            return true;
        }
        else if (other.Chapter.Number < Descriptor.Chapter.Number)
        {
            return false;
        }
#if DEBUG
        if (other.Chapter != Descriptor.Chapter)
        {
            throw new InvalidOperationException($"ChapterDescriptor number mismatch between either {other.Chapter} or {Descriptor.Chapter}");
        }
#endif

        if (Descriptor.CheckpointId.CheckpointType == CheckpointTypes.MainPath
            && other.CheckpointId.CheckpointType == CheckpointTypes.MainPath)
        {
            return other.CheckpointId.CheckpointNumber > Descriptor.CheckpointId.CheckpointNumber;
        }

        return false;
    }

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