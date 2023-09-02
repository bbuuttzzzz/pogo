using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Checkpoints
{
    [System.Serializable]
    public enum CheckpointTypes
    {
        MainPath,
        SidePath
    }

    [System.Serializable]
    public struct CheckpointId
    {
        public CheckpointTypes CheckpointType;
        public int CheckpointNumber;

        public CheckpointId(CheckpointTypes checkpointType, int checkpointNumber)
        {
            CheckpointType = checkpointType;
            CheckpointNumber = checkpointNumber;
        }

        public override string ToString()
        {
            return $"{CheckpointType} {CheckpointNumber}";
        }
    }
}
