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
        public Transform Point;
        public CheckpointTrigger Trigger;

        public RespawnPointData(Transform point, CheckpointTrigger trigger)
        {
            Point = point;
            Trigger = trigger;
        }
    }
}
