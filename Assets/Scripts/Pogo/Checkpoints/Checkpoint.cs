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
        Transform ICheckpoint.SpawnPoint => RespawnPoint;
        public Transform RespawnPoint;
        public abstract bool CanSkip { get; set; }

        [HideInInspector]
        public UnityEvent OnSkip;

        CheckpointId ICheckpoint.Id => CheckpointId;
        UnityEvent ICheckpoint.OnSkip => OnSkip;
    }
}
