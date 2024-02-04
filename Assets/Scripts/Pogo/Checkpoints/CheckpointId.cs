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
    public struct CheckpointId : IEquatable<CheckpointId>
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

        #region Equals
        public bool Equals(CheckpointId other)
        {
            return other.CheckpointType == CheckpointType
                && other.CheckpointNumber == CheckpointNumber;
        }

        public static bool operator ==(CheckpointId left, CheckpointId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CheckpointId left, CheckpointId right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is CheckpointId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
#endregion
    }
}
