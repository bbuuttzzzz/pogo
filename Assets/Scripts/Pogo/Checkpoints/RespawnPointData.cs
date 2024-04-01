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
        public ICheckpoint Checkpoint;
        public CustomCheckpointController CustomCheckpoint;
        public LevelDescriptor OverrideLevel;
        public Vector3 InitialVelocity;

        public RespawnPointData(Transform transform, LevelDescriptor overrideLevel = null)
        {
            this.transform = transform;
            Checkpoint = null;
            CustomCheckpoint = null;
            OverrideLevel = overrideLevel;
            InitialVelocity = Vector3.zero;
        }

        public RespawnPointData(ICheckpoint checkpoint, LevelDescriptor overrideLevel = null)
        {
            transform = checkpoint.SpawnPoint;
            Checkpoint = checkpoint;
            CustomCheckpoint = null;
            OverrideLevel = overrideLevel;
            InitialVelocity = checkpoint.InitialVelocity;
        }

        public RespawnPointData(CustomCheckpointController customCheckpoint, LevelDescriptor overrideLevel = null)
        {
            transform = customCheckpoint.transform;
            Checkpoint = null;
            CustomCheckpoint = customCheckpoint;
            OverrideLevel = overrideLevel;
            InitialVelocity = Vector3.zero;
        }
    }
}
