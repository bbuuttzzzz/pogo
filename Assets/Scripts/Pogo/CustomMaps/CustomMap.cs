using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps
{
    public class CustomMap
    {
        public Dictionary<CheckpointId, GeneratedCheckpoint> Checkpoints;

        public CustomMap()
        {
            Checkpoints = new Dictionary<CheckpointId, GeneratedCheckpoint>();
        }

        public void RegisterCheckpoint(GeneratedCheckpoint checkpoint)
        {
            Checkpoints.Add(checkpoint.Id, checkpoint);
        }
    }
}
