using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct QuickSaveData
    {
        [System.Serializable]
        public enum States
        {
            NoData,
            InProgress
        }
        public States CurrentState;
        public ChapterId ChapterId;
        public CheckpointId checkpointId;
        public int TrackedDeaths;
        public int ElapsedMilliseconds;
    }
}
