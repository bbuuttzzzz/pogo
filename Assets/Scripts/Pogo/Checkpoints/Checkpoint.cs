using Pogo.Checkpoints;
using System;
using UnityEngine;

namespace Pogo
{
    public abstract class Checkpoint : Trigger, ICheckpoint
    {
        public abstract ChapterDescriptor Chapter { get; }
        public abstract CheckpointId CheckpointId { get; }
        Transform ICheckpoint.SpawnPoint => RespawnPoint;
        public Transform RespawnPoint;

        public abstract bool CanSkip { get; set; }

        CheckpointId ICheckpoint.Id => CheckpointId;

        [Serializable]
        public enum SkipBehaviors
        {
            LevelChange,
            TeleportToTarget,
            HalfCheckpoint
        }
        [HideInInspector]
        public SkipBehaviors SkipBehavior;
    }
}
