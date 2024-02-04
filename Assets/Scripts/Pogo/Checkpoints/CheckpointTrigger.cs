using Pogo.Checkpoints;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class CheckpointTrigger : Trigger
    {
        public CheckpointDescriptor Descriptor;
        public Transform RespawnPoint;
        public UnityEvent OnEnteredNotActivated;

        [Serializable]
        public enum SkipBehaviors
        {
            LevelChange,
            TeleportToTarget,
            HalfCheckpoint
        }
        [HideInInspector]
        public SkipBehaviors SkipBehavior;

        [HideInInspector]
        public UnityEvent OnSkip;

        [HideInInspector]
        public Transform SkipTarget;

        public void Awake()
        {
            PogoGameManager.PogoInstance.LoadCheckpointManifest.Add(this);
        }

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

        private void OnDestroy()
        {
            PogoGameManager.PogoInstance.LoadCheckpointManifest.Remove(this);
        }
    }
}