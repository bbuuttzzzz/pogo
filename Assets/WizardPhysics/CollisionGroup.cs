using System.Collections.Generic;
using UnityEngine;
using WizardUtils;

namespace WizardPhysics
{
    public class CollisionGroup : MonoBehaviour
    {
        public CollisionOrb[] Group;
        public float skinWidth = 0.01f;

        public void Move(Vector3 movement, CollisionEvent onCollide = null)
        {
            Vector3 remainingMovement = movement;
            while (remainingMovement.magnitude >= 0.003f)
            {
                float distance = remainingMovement.magnitude;
                TestResult firstCollision = null;

                // test all orbs to find closest collision
                for (int orbIndex = 0; orbIndex < Group.Length; orbIndex++)
                {
                    CollisionOrb orb = Group[orbIndex];
                    if (orb.TestRay(new Ray(orb.transform.position, remainingMovement), distance + skinWidth, out RaycastHit hit))
                    {
                        if (firstCollision == null || hit.distance < firstCollision.HitInfo.distance)
                        {
                            firstCollision = new TestResult(hit, orb);
                        }
                    }
                }

                if (firstCollision != null)
                {
                    // move up to the object
                    transform.position += (firstCollision.HitInfo.distance - skinWidth) * remainingMovement.normalized;
                    remainingMovement -= (firstCollision.HitInfo.distance - skinWidth) * remainingMovement.normalized;

                    // remove non-tangent component from remainingMovement
                    remainingMovement = remainingMovement.GetTangentComponent(firstCollision.HitInfo.normal);

                    // call collision events
                    var args = new CollisionEventArgs(firstCollision.HitInfo);
                    onCollide.Invoke(args);
                    firstCollision.CollidedOrb.OnCollisionEnter.Invoke(args);
                }
            }
        }

        public PositionRecording RecordPosition()
        {
            return new PositionRecording(transform, Group);
        }

        public class PositionRecording
        {
            public Vector3[] OrbPositions;
            public Vector3 ParentPosition;
            public Quaternion ParentRotation;

            public PositionRecording(Transform parent, CollisionOrb[] children)
            {
                ParentPosition = parent.position;
                ParentRotation = parent.rotation;

                OrbPositions = new Vector3[children.Length];
                for (int n = 0; n < children.Length; n++)
                {
                    OrbPositions[n] = children[n].transform.position;
                }
            }
        }

        class TestResult
        {
            public RaycastHit HitInfo;
            public CollisionOrb CollidedOrb;

            public TestResult(RaycastHit hitInfo, CollisionOrb collidedOrb)
            {
                this.HitInfo = hitInfo;
                this.CollidedOrb = collidedOrb;
            }
        }
    }
}
