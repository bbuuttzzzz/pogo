using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps
{
    public class CustomMap
    {
        public Dictionary<CheckpointId, GeneratedCheckpoint> Checkpoints;
        public GameObject PlayerStart;
        public GeneratedCheckpoint FirstCheckpoint;

        public CustomMap()
        {
            Checkpoints = new Dictionary<CheckpointId, GeneratedCheckpoint>();
        }

        public void RegisterCheckpoint(GeneratedCheckpoint checkpoint)
        {
            Checkpoints.Add(checkpoint.Id, checkpoint);

            if (checkpoint.CheckpointId.CheckpointType == CheckpointTypes.MainPath
                && (FirstCheckpoint == null || checkpoint.Id.CheckpointNumber < FirstCheckpoint.CheckpointId.CheckpointNumber))
            {
                FirstCheckpoint = checkpoint;
            }
        }
    }
}
