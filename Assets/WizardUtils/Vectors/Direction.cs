using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Vectors
{
    [Serializable]
    public class Direction : IEquatable<Direction>
    {
        [SerializeField]
        Vector3Int vector;
        public Vector3Int Vector
        {
            get => vector;
            set
            {
                if (!IsValidVector(value)) throw new ArgumentOutOfRangeException();
                vector = value;
            }
        }
        public int x => Vector.x;
        public int y => Vector.y;
        public int z => Vector.z;

        public Direction()
        {

        }

        public Direction(Vector3Int vector) : base()
        {
            Vector = vector;
        }

        public Direction(Vector3 fuzzyDirection) : base()
        {
            Vector = new Vector3Int
            {
                x = Mathf.RoundToInt(fuzzyDirection.x),
                y = Mathf.RoundToInt(fuzzyDirection.y),
                z = Mathf.RoundToInt(fuzzyDirection.z),
            };
        }

        public static bool IsValidVector(Vector3Int vector)
        {
            return vector.magnitude == 1;
        }

        /// <summary>
        /// Rotate this GridEdge around the origin counter clockwise by specified degree
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Direction Rotate(RotationType rotation)
        {
            if (this == Direction.up || this == Direction.down)
            {
                return this;
            }

            // Convert our initial direction to 0-4 turns from forward
            // add to that 0-4 turns
            int turns = (toTurns() + RotationHelper.rotationToTurns(rotation)) % 4;

            // convert back to direction
            return fromTurns(turns);
        }

        public RotationType GetRotationFrom(Direction origin)
        {
            return RotationHelper.turnsToRotation(this.countTurns(origin));
        }
        int countTurns(Direction origin)
        {
            if (this == Direction.up || this == Direction.down
                || origin == Direction.up || origin == Direction.down)
            {
                throw new ArgumentOutOfRangeException();
            }

            int turns = this.toTurns() - origin.toTurns();

            return (turns + 8) % 4;
        }

        int toTurns()
        {
            return (this == Direction.right) ? 1
                : (this == Direction.back) ? 2
                : (this == Direction.left) ? 3
                : 0;
        }
        Direction fromTurns(int turns)
        {
            return (turns == 1 ? Direction.right
                : turns == 2 ? Direction.back
                : turns == 3 ? Direction.left
                : Direction.forward);
        }

        const float oneOverRoot2 = 0.707107f;
        public Quaternion GetQuaternionFromForward()
        {
            return vector == Vector3Int.up ? new Quaternion(-oneOverRoot2, 0, 0, oneOverRoot2)
                : vector == Vector3Int.down ? new Quaternion(oneOverRoot2, 0, 0, oneOverRoot2)
                : vector == Vector3Int.left ? new Quaternion(0, -oneOverRoot2, 0, oneOverRoot2)
                : vector == Vector3Int.right ? new Quaternion(0, oneOverRoot2, 0, oneOverRoot2)
                : vector == Vector3Int.back ? new Quaternion(0, -1, 0, 0)
                : Quaternion.identity;
        }
        #region Constants
        public static Direction up { get => new Direction(Vector3Int.up); }
        public static Direction down { get => new Direction(Vector3Int.down); }
        public static Direction left { get => new Direction(Vector3Int.left); }
        public static Direction right { get => new Direction(Vector3Int.right); }
        public static Direction forward { get => new Direction(new Vector3Int(0, 0, 1)); }
        public static Direction back { get => new Direction(new Vector3Int(0, 0, -1)); }

        public static string[] names => new string[]
        {
            nameof(up),
            nameof(down),
            nameof(left),
            nameof(right),
            nameof(forward),
            nameof(back)
        };
        public static Direction[] all => new Direction[] { up, down, left, right, forward, back };
        #endregion

        #region C# Functions

        public override int GetHashCode()
        {
            return Vector.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is Direction) return Equals((Direction)obj);
            return false;
        }
        public bool Equals(Direction other)
        {
            return Vector.Equals(other.Vector);
        }
        public static bool operator ==(Direction a, Direction b)
        {
            return a.Vector.Equals(b.Vector);
        }
        public static bool operator !=(Direction a, Direction b)
        {
            return !a.Vector.Equals(b.Vector);
        }
        public override string ToString()
        {
            if (this == Direction.up) return "Up";
            else if (this == Direction.down) return "Down";
            else if (this == Direction.left) return "Left";
            else if (this == Direction.right) return "Right";
            else if (this == Direction.back) return "Back";
            else if (this == Direction.forward) return "Forward";
            else return "Invalid";
        }

        #endregion
    }
}
