using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Checkpoints
{
    public struct RespawnPointData
    {
        public Transform transform;
        public CheckpointTrigger Trigger;
        public CustomCheckpointController CustomCheckpoint;

        public RespawnPointData(Transform transform)
        {
            this.transform = transform;
            Trigger = null;
            CustomCheckpoint = null;
        }

        public RespawnPointData(CheckpointTrigger trigger)
        {
            transform = trigger.RespawnPoint;
            Trigger = trigger;
            CustomCheckpoint = null;
        }

        public RespawnPointData(CustomCheckpointController customCheckpoint)
        {
            transform = customCheckpoint.transform;
            Trigger = null;
            CustomCheckpoint = customCheckpoint;
        }
    }
}
