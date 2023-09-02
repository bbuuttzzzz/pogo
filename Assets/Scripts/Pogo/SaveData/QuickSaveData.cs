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
        public enum States
        {
            NoData,
            InProgress
        }
        public int ChapterIndex;
        public CheckpointId checkpointId;
        public int TrackedDeaths;
        public int ElapsedMilliseconds;
    }
}
