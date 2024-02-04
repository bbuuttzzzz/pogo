using Pogo.Levels;
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
        public ExplicitCheckpoint Trigger;
        public CustomCheckpointController CustomCheckpoint;
        public LevelDescriptor OverrideLevel;

        public RespawnPointData(Transform transform, LevelDescriptor overrideLevel = null)
        {
            this.transform = transform;
            Trigger = null;
            CustomCheckpoint = null;
            OverrideLevel = overrideLevel;
        }

        public RespawnPointData(ExplicitCheckpoint trigger, LevelDescriptor overrideLevel = null)
        {
            transform = trigger.RespawnPoint;
            Trigger = trigger;
            CustomCheckpoint = null;
            OverrideLevel = overrideLevel;
        }

        public RespawnPointData(CustomCheckpointController customCheckpoint, LevelDescriptor overrideLevel = null)
        {
            transform = customCheckpoint.transform;
            Trigger = null;
            CustomCheckpoint = customCheckpoint;
            OverrideLevel = overrideLevel;
        }
    }
}
