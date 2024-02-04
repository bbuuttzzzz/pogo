using Pogo.Checkpoints;
using UnityEngine;

namespace Pogo
{
    public abstract class Checkpoint : Trigger, ICheckpoint
    {
        public abstract ChapterDescriptor Chapter { get; }
        public abstract CheckpointId CheckpointId { get; }
        public Transform RespawnPoint { get; }

        CheckpointId ICheckpoint.Id => CheckpointId;
    }
}
