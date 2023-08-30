using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct ChapterId : IEquatable<ChapterId>
    {
        public int WorldNumber;
        public int ChapterNumber;

        public override bool Equals(object obj)
        {
            return obj is ChapterId other && Equals(other);
        }

        public bool Equals(ChapterId other)
        {
            return WorldNumber == other.WorldNumber &&
                ChapterNumber == other.ChapterNumber;
        }

        public static bool operator ==(ChapterId a, ChapterId b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ChapterId a, ChapterId b)
        {
            return !a.Equals(b);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(WorldNumber, ChapterNumber);
        }
    }
}
