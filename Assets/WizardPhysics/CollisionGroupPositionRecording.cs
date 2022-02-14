using UnityEngine;

namespace WizardPhysics
{
    public class CollisionGroupPositionRecording
    {
        public Vector3[] OrbPositions;
        public Vector3 ParentPosition;
        public Quaternion ParentRotation;

        public CollisionGroupPositionRecording(CollisionGroupPositionRecording recording)
        {
            this.OrbPositions = new Vector3[recording.OrbPositions.Length];
            System.Array.Copy(recording.OrbPositions, this.OrbPositions, recording.OrbPositions.Length);
            this.ParentPosition = recording.ParentPosition;
            this.ParentRotation = recording.ParentRotation;
        }

        public CollisionGroupPositionRecording(Transform parent, Transform swivel, CollisionOrb[] children)
        {
            ParentPosition = parent.position;
            ParentRotation = swivel.rotation;

            OrbPositions = new Vector3[children.Length];
            for (int n = 0; n < children.Length; n++)
            {
                OrbPositions[n] = children[n].transform.position;
            }
        }

        public CollisionGroupPositionRecording Rotate(Quaternion rotation)
        {
            CollisionGroupPositionRecording newRecording = new CollisionGroupPositionRecording(this);

            for (int n = 0; n < OrbPositions.Length; n++)
            {
                newRecording.OrbPositions[n] = rotation * (OrbPositions[n] - ParentPosition) + ParentPosition;
            }

            newRecording.ParentRotation *= rotation;

            return newRecording;
        }

        public CollisionGroupPositionRecording RotateAround(Vector3 origin, Quaternion rotation)
        {
            CollisionGroupPositionRecording newRecording = new CollisionGroupPositionRecording(this);

            for (int n = 0; n < OrbPositions.Length; n++)
            {
                newRecording.OrbPositions[n] = rotation * (OrbPositions[n] - origin) + origin;
            }

            newRecording.ParentPosition = rotation * (newRecording.ParentPosition - origin) + origin;
            newRecording.ParentRotation *= rotation;

            return newRecording;
        }

        public CollisionGroupPositionRecording Translate(Vector3 movement)
        {
            CollisionGroupPositionRecording newRecording = new CollisionGroupPositionRecording(this);

            for (int n = 0; n < OrbPositions.Length; n++)
            {
                newRecording.OrbPositions[n] += movement;
            }

            newRecording.ParentPosition += movement;

            return newRecording;
        }
    }
}
