using System;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils;
using WizardUtils.Math;

namespace WizardPhysics
{
    public class CollisionGroup : MonoBehaviour
    {
        public CollisionOrb[] CollisionOrbs;
        public float skinWidth = 0.01f;

        [Tooltip("Instead of rotating this transform, rotate this specific transform (must be a child)")]
        public Transform SwivelTransform;

        public CollisionEvent OnCollide;

        private void Awake()
        {
            if (SwivelTransform == null) SwivelTransform = transform;

#if UNITY_EDITOR
            foreach(CollisionOrb orb in CollisionOrbs)
            {
                if (!orb.transform.IsChildOf(SwivelTransform))
                {
                    Debug.LogWarning($"{orb.gameObject} isn't a child of CollisionGoup {gameObject}");
                }
            }
#endif
        }

        public void RotateTo(Quaternion targetRotation)
        {
            Rotate(Quaternion.Inverse(SwivelTransform.rotation) * targetRotation);
        }

        public void Rotate(Quaternion addRotation)
        {
            // rotate the full distance and check for collisions
            PositionRecording start = new PositionRecording(transform, SwivelTransform, CollisionOrbs);
            PositionRecording fullRotation = start.Rotate(addRotation);
            TestResult firstCollision = FindFirstCollision(start, fullRotation);

            if (firstCollision == null)
            {
                // we hit nothing. just move forward
                SwivelTransform.rotation *= addRotation;
                return;
            }

            // find the point we hit rotating instead of just lerping
            int pivotIndex = FindOrb(firstCollision.CollidedOrb);
            addRotation.ToAngleAxis(out float addAngle, out Vector3 addAxis);
            Vector3 rotateForwardCollisionPoint = GetRealCollisionPoint(addRotation, start, firstCollision, pivotIndex);
            if (rotateForwardCollisionPoint.x == float.NaN)
            {
                // we should really have come into contact with the same plane... if this happened theres probably something wrong with GetRealCollisionPoint
                Debug.LogWarning("IntersectCircle missed... that means use a new algorithm");
                ApplyRecording(fullRotation);
            }

            // rotate forward to the collision point 
            float firstAngle = Vector3.Angle(start.OrbPositions[pivotIndex] - start.ParentPosition, rotateForwardCollisionPoint - start.ParentPosition);
            Quaternion firstRotation = Quaternion.AngleAxis(firstAngle, addAxis);
            PositionRecording rotatedToCollision = start.Rotate(firstRotation);

            // try rotating backwards the rest of the way
            Quaternion backRotation = Quaternion.AngleAxis(addAngle - firstAngle, addAxis);
            PositionRecording backRotated = rotatedToCollision.RotateAround(rotatedToCollision.OrbPositions[pivotIndex], backRotation);
            bool backRotationDidHit = TestForCollision(start, backRotated, pivotIndex);

            if (backRotationDidHit)
            {
                // just clip into the original wall. oh well.
                Debug.Log("CollisionGroup Rotation Clipped.");
                ApplyRecording(fullRotation);
                return;
            }

            //rotate forward until we hit the first surface, and then rotate back the remainder of the way
            ApplyRecording(backRotated);

            // call collision events
            var args = new CollisionEventArgs(firstCollision.HitInfo);
            OnCollide.Invoke(args);
            firstCollision.CollidedOrb.OnCollisionEnter.Invoke(args);
        }

        public void Move(Vector3 movement)
        {
            Vector3 remainingMovement = movement;
            while (remainingMovement.magnitude >= 0.003f)
            {
                float distance = remainingMovement.magnitude;
                TestResult firstCollision = null;

                // test all orbs to find closest collision
                for (int orbIndex = 0; orbIndex < CollisionOrbs.Length; orbIndex++)
                {
                    CollisionOrb orb = CollisionOrbs[orbIndex];
                    Ray ray = new Ray(orb.transform.position, remainingMovement);
                    if (orb.TestRay(ray, distance + skinWidth, out RaycastHit hit))
                    {
                        if (firstCollision == null || hit.distance < firstCollision.MaxDistance)
                        {
                            firstCollision = new TestResult(hit, orb, ray, distance + skinWidth);
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
                    OnCollide.Invoke(args);
                    firstCollision.CollidedOrb.OnCollisionEnter.Invoke(args);
                }
            }
        }

        #region Helper Functions
        private void ApplyRecording(PositionRecording recording)
        {
            transform.position = recording.ParentPosition;
            SwivelTransform.rotation = recording.ParentRotation;
        }


        private bool TestForCollision(PositionRecording start, PositionRecording end, int ignoreThisIndex = -1)
        {
            for (int n = 0; n < CollisionOrbs.Length; n++)
            {
                Vector3 direction = end.OrbPositions[n] - start.OrbPositions[n];
                float distance = direction.magnitude;
                Ray testRay = new Ray(start.OrbPositions[n], direction);
                if (CollisionOrbs[n].TestRay(testRay, distance + skinWidth, out RaycastHit hit))
                {
                    return true;
                }
            }

            return false;
        }

        private TestResult FindFirstCollision(PositionRecording start, PositionRecording end)
        {
            TestResult firstCollision = null;

            // test all orbs to find closest collision
            for (int n = 0; n < CollisionOrbs.Length; n++)
            {
                Vector3 direction = end.OrbPositions[n] - start.OrbPositions[n];
                float distance = direction.magnitude;
                Ray testRay = new Ray(start.OrbPositions[n], direction);
                if (CollisionOrbs[n].TestRay(testRay, distance + skinWidth, out RaycastHit hit))
                {
                    float maxDistance = (distance + skinWidth);
                    if (firstCollision == null || hit.distance / maxDistance < firstCollision.HitInfo.distance / firstCollision.MaxDistance)
                    {
                        firstCollision = new TestResult(hit, CollisionOrbs[n], testRay, maxDistance);
                    }
                }
            }

            return firstCollision;
        }

        private Vector3 GetRealCollisionPoint(Quaternion addRotation, PositionRecording start, TestResult firstCollision, int pivotIndex)
        {
            // we need to look for the intersection point of a sphere and a plane
            // that's the same as the intersection of a point and a plane
            // ...if the plane is shifted forward by 1 radius
            Vector3 planeOrigin = firstCollision.HitInfo.point + firstCollision.HitInfo.normal * (CollisionOrbs[pivotIndex].Radius + skinWidth);

            // get the radius of our circle by orb's distance from the pivot point
            float radius = (start.OrbPositions[pivotIndex] - start.ParentPosition).magnitude;

            // create a new coordinate plane
            CoordinateSystem2D rotSystem = new CoordinateSystem2D(
                addRotation * Vector3.right,
                addRotation * Vector3.up
                );

            // find 2 points on the intersection of the planes
            Vector3 intersection1 = planeOrigin; // that was easy
            Vector3 intersection2 = intersection1 + Vector3.Cross(firstCollision.HitInfo.normal, addRotation * Vector3.right);

            // project center of circle onto the new coordinate plane
            Vector2 rotCircleCenter = rotSystem.Project(start.ParentPosition);

            // project intersection onto the new coordinate plane as a line, offset to put circle at center
            StandardLine2D rotLine = new StandardLine2D(
                rotSystem.Project(intersection1) - rotCircleCenter,
                rotSystem.Project(intersection2) - rotCircleCenter
                );

            Vector2[] localRotIntersects = GeometryHelper.IntersectCircleLine(rotLine, radius);

            Vector3 closestPoint = new Vector3(float.NaN, float.NaN, float.NaN);
            foreach (Vector2 localRotIntersect in localRotIntersects)
            {
                Vector3 intersectPoint = rotSystem.Compose(localRotIntersect + rotCircleCenter);

                if (closestPoint.x == float.NaN || intersectPoint.sqrMagnitude < closestPoint.sqrMagnitude)
                {
                    closestPoint = intersectPoint;
                }
            }

            return closestPoint;
        }

        internal void RotateTo(Quaternion desiredModelRotation, object onCollide)
        {
            throw new NotImplementedException();
        }

        private int FindOrb(CollisionOrb orb)
        {
            for (int n = 0; n < CollisionOrbs.Length; n++)
            {
                if (orb == CollisionOrbs[n]) return n;
            }
            throw new MissingMemberException();
        }
    #endregion

        public class PositionRecording
        {
            public Vector3[] OrbPositions;
            public Vector3 ParentPosition;
            public Quaternion ParentRotation;

            public PositionRecording(PositionRecording recording)
            {
                this.OrbPositions = recording.OrbPositions;
                this.ParentPosition = recording.ParentPosition;
                this.ParentRotation = recording.ParentRotation;
            }

            public PositionRecording(Transform parent, Transform swivel, CollisionOrb[] children)
            {
                ParentPosition = parent.position;
                ParentRotation = swivel.rotation;

                OrbPositions = new Vector3[children.Length];
                for (int n = 0; n < children.Length; n++)
                {
                    OrbPositions[n] = children[n].transform.position;
                }
            }

            public PositionRecording Rotate(Quaternion rotation)
            {
                PositionRecording newRecording = new PositionRecording(this);

                for (int n = 0; n < OrbPositions.Length; n++)
                {
                    newRecording.OrbPositions[n] = rotation * (OrbPositions[n] - ParentPosition) + ParentPosition;
                }

                newRecording.ParentRotation *= rotation;

                return newRecording;
            }

            public PositionRecording RotateAround(Vector3 origin, Quaternion rotation)
            {
                PositionRecording newRecording = new PositionRecording(this);

                for (int n = 0; n < OrbPositions.Length; n++)
                {
                    newRecording.OrbPositions[n] = rotation * (OrbPositions[n] - origin) + origin;
                }

                newRecording.ParentPosition = rotation * (newRecording.ParentPosition - origin) + origin;
                newRecording.ParentRotation *= rotation;

                return newRecording;
            }

            internal PositionRecording Translate(Vector3 movement)
            {
                PositionRecording newRecording = new PositionRecording(this);

                for (int n = 0; n < OrbPositions.Length; n++)
                {
                    newRecording.OrbPositions[n] += movement;
                }

                newRecording.ParentPosition += movement;

                return newRecording;
            }
        }

        class TestResult
        {
            public RaycastHit HitInfo;
            public CollisionOrb CollidedOrb;
            public Ray TestRay;
            public float MaxDistance;


            public TestResult(RaycastHit hitInfo, CollisionOrb collidedOrb, Ray testRay, float maxDistance)
            {
                this.HitInfo = hitInfo;
                this.CollidedOrb = collidedOrb;
                this.TestRay = testRay;
                this.MaxDistance = maxDistance;
            }
        }
    }
}
