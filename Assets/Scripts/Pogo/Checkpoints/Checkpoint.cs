using Pogo.Checkpoints;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public abstract class Checkpoint : Trigger, ICheckpoint
    {
        public abstract ChapterDescriptor Chapter { get; }
        public abstract CheckpointId CheckpointId { get; }

        Vector3 ICheckpoint.InitialVelocity => InitialVelocity;
        public Vector3 InitialVelocity;
        Transform ICheckpoint.SpawnPoint => RespawnPoint;
        public Transform RespawnPoint;
        public abstract bool CanSkip { get; set; }
        public UnityEvent OnEnteredNotActivated;

        [HideInInspector]
        public UnityEvent OnSkip;

        CheckpointId ICheckpoint.Id => CheckpointId;
        UnityEvent ICheckpoint.OnSkip => OnSkip;

        [HideInInspector]
        public SkipBehaviors SkipBehavior;

        [HideInInspector]
        public Transform SkipTarget;

        public override bool ColliderCanTrigger(Collider other)
        {
            if (WizardUtils.GameManager.GameInstanceIsValid())
            {
                bool success = PogoGameManager.PogoInstance.TryRegisterRespawnPoint(this);
                if (!success) OnEnteredNotActivated?.Invoke();
                return success;
            }
            else
                return false;
        }
    }
}
