using System;
using System.Collections.Generic;

namespace Pogo.Levels
{
    [System.Serializable]
    public struct LevelState : IEquatable<LevelState>
    {
        public LevelDescriptor Level;
        public int StateId;

        public const int State_Any = -1;

        public bool AnyState
        {
            get => StateId == State_Any;
            set
            {
                if (value == AnyState) return;
                StateId = value ? State_Any : 0;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is LevelState state && Equals(state);
        }

        public bool Equals(LevelState other)
        {
            return EqualityComparer<LevelDescriptor>.Default.Equals(Level, other.Level) &&
                   StateId == other.StateId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Level, StateId);
        }

        public override string ToString()
        {
            return $"{Level.name} State {StateId}";
        }

        public static bool operator ==(LevelState left, LevelState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LevelState left, LevelState right)
        {
            return !(left == right);
        }
    }
}