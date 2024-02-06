using Pogo.Checkpoints;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class ExplicitCheckpoint : Checkpoint
    {
        public CheckpointDescriptor Descriptor;
        public UnityEvent OnEnteredNotActivated;

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

        public override ChapterDescriptor Chapter => Descriptor.Chapter;

        public override CheckpointId CheckpointId => Descriptor.CheckpointId;
        public override bool CanSkip
        {
            get => Descriptor.CanSkip;
            set
            {
                Descriptor.CanSkip = value;
            }
        }
    }
}