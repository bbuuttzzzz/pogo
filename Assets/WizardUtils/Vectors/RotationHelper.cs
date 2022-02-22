using UnityEngine;


namespace WizardUtils.Vectors
{
    public enum RotationType { r0, r90, r180, r270 }

    public static class RotationHelper
    {
        public static int rotationToTurns(RotationType rotation)
        {
            return (rotation == RotationType.r90 ? 1
                : rotation == RotationType.r180 ? 2
                : rotation == RotationType.r270 ? 3
                : 0);
        }
        public static RotationType turnsToRotation(int turns)
        {
            return (turns == 1 ? RotationType.r90
                : turns == 2 ? RotationType.r180
                : turns == 3 ? RotationType.r270
                : RotationType.r0);
        }

        public static int ToAngle(this RotationType self)
        {
            switch (self)
            {
                case RotationType.r90:
                    return 90;
                case RotationType.r180:
                    return 180;
                case RotationType.r270:
                    return 270;
                default:
                    return 0;
            }
        }

        public static Vector3Int ToLevelSpace(this Vector3Int localPosition, Vector3Int origin, RotationType rotation)
        {
            return origin + localPosition.Rotate(rotation);
        }

        public static Vector3Int Rotate(this Vector3Int position, RotationType rotation)
        {
            Vector3Int newPosition = position;
            
            //TODO: Matrix Math optimization
            int turns = RotationHelper.rotationToTurns(rotation);
            if (turns % 2 == 1)
            {
                int mirror = turns == 3 ? -1 : 1;
                newPosition = new Vector3Int
                {
                    x = newPosition.z * mirror,
                    y = newPosition.y,
                    z = newPosition.x * -1 * mirror
                };
            }
            else
            {
                int mirror = turns == 2 ? -1 : 1;
                newPosition = new Vector3Int
                {
                    x = newPosition.x * mirror,
                    y = newPosition.y,
                    z = newPosition.z * mirror
                };
            }

            return newPosition;
        }
    }
}
