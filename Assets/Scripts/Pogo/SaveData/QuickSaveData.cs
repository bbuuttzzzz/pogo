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
        public int ShareIndex;
        public int CheckpointNumber;
        public int Deaths;
        public int Time;
    }
}
