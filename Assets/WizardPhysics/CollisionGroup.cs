using System;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils;
using WizardUtils.Math;

namespace WizardPhysics
{
    public partial class CollisionGroup : MonoBehaviour
    {
        public CollisionOrb[] CollisionOrbs;
        public float skinWidth = 0.01f;
        float rotationSkinDegrees;

        [Tooltip("Instead of rotating this transform, rotate this specific transform (must be a child)")]
        public Transform SwivelTransform;

        public CollisionEvent OnCollide;

        public bool DrawDebugGizmos;

        private void Awake()
        {
            if (SwivelTransform == null) SwivelTransform = transform;

            float minRadius = float.MaxValue;
            foreach (CollisionOrb orb in CollisionOrbs)
            {
#if UNITY_EDITOR
                if (!orb.transform.IsChildOf(SwivelTransform))
                {
                    Debug.LogWarning($"{orb.gameObject} isn't a child of CollisionGoup {gameObject}");
                }
#endif
                minRadius = Mathf.Min(minRadius, (orb.transform.position - transform.position).magnitude);
            }

            rotationSkinDegrees = 360 * skinWidth / (2 * Mathf.PI * minRadius);

            OnCollide.AddListener(checkForTriggers);
        }

        public void RotateTo(Quaternion targetRotation)
        {
            Rotate(Quaternion.Inverse(SwivelTransform.rotation) * targetRotation);
        }

        public void Rotate(Quaternion addRotation)
        {
            // rotate the full distance (+ skin) and check for collisions
            addRotation.ToAngleAxis(out float addAngle, out Vector3 addAxis);
            if (addAngle < 1E-8f) return;

            Quaternion skinnedAddRotation = Quaternion.AngleAxis(addAngle + rotationSkinDegrees, addAxis);
            CollisionGroupPositionRecording start = new CollisionGroupPositionRecording(transform, SwivelTransform, CollisionOrbs);
            CollisionGroupPositionRecording skinnedFullRotation = start.Rotate(skinnedAddRotation);
            TestResult firstCollision = FindFirstCollision(start, skinnedFullRotation);


            if (firstCollision == null)
            {
                // we hit nothing. just move forward
                SwivelTransform.rotation *= addRotation;
                return;
            }

            // see if we can still do our full rotation against this surface
            int index = FindOrb(firstCollision.CollidedOrb);
            CollisionGroupPositionRecording final = start.Rotate(addRotation);
            float newHeight = Vector3.Dot(firstCollision.HitInfo.normal, final.OrbPositions[index] - firstCollision.HitInfo.point);
            float addHeight = newHeight - firstCollision.CollidedOrb.Radius - skinWidth;
            Vector3 translation;

            if (addHeight > 0)
            {
                translation = firstCollision.HitInfo.normal * addHeight;
                final = start.Translate(translation);
            }
            else
            {
                translation = Vector3.zero;
            }

            // call collision events
            var firstCollisionArgs = new CollisionEventArgs(firstCollision.HitInfo);
            OnCollide.Invoke(firstCollisionArgs);
            firstCollision.CollidedOrb.OnCollisionEnter.Invoke(firstCollisionArgs);

            transform.position += translation;
            SwivelTransform.rotation *= addRotation;
            return;
        }

        public void Move(Vector3 movement)
        {
            Vector3 remainingMovement = movement;
            while (remainingMovement.magnitude >= 1E-8f)
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
                    // move up to the object, minus a skin width from the surface
                    float angleOfIncidence = Vector3.Angle(firstCollision.HitInfo.normal, remainingMovement) - 90;
                    float skinDepth = skinWidth / Mathf.Sin(angleOfIncidence * Mathf.Deg2Rad);

                    float moveDistance = firstCollision.HitInfo.distance - skinDepth;
                    transform.position += moveDistance * remainingMovement.normalized;
                    remainingMovement -= moveDistance * remainingMovement.normalized;

                    // remove non-tangent component from remainingMovement
                    remainingMovement = remainingMovement.GetTangentComponent(firstCollision.HitInfo.normal);

                    // call collision events
                    var args = new CollisionEventArgs(firstCollision.HitInfo);
                    OnCollide.Invoke(args);
                    firstCollision.CollidedOrb.OnCollisionEnter.Invoke(args);
                }
                else
                {
                    // move all the way up
                    transform.position += remainingMovement;
                    remainingMovement = Vector3.zero;
                }
            }
        }

        private const int SafeLayer = 30;
        private const int SafeLayerMask = (1 << SafeLayer);

        public void TestAgainst(Collider other, Vector3 movement)
        {
            using var _ = new LayerChangeScope(other.gameObject, SafeLayer);
            Vector3 remainingMovement = movement;
            Vector3 initialPosition = transform.position;

            while (remainingMovement.magnitude >= 1E-8f)
            {
                float distance = remainingMovement.magnitude;
                TestResult firstCollision = null;

                // test all orbs to find closest collision
                for (int orbIndex = 0; orbIndex < CollisionOrbs.Length; orbIndex++)
                {
                    CollisionOrb orb = CollisionOrbs[orbIndex];
                    Ray ray = new Ray(orb.transform.position, remainingMovement);
                    if (orb.TestRay(ray, distance + skinWidth, out RaycastHit hit, SafeLayerMask))
                    {
                        if (firstCollision == null || hit.distance < firstCollision.MaxDistance)
                        {
                            firstCollision = new TestResult(hit, orb, ray, distance + skinWidth);
                        }
                    }
                }

                if (firstCollision != null)
                {
                    // move up to the object, minus a skin width from the surface
                    float angleOfIncidence = Vector3.Angle(firstCollision.HitInfo.normal, remainingMovement) - 90;
                    float skinDepth = skinWidth / Mathf.Sin(angleOfIncidence * Mathf.Deg2Rad);

                    float moveDistance = firstCollision.HitInfo.distance - skinDepth;
                    transform.position += moveDistance * remainingMovement.normalized;
                    remainingMovement -= moveDistance * remainingMovement.normalized;

                    // remove non-tangent component from remainingMovement
                    remainingMovement = remainingMovement.GetTangentComponent(firstCollision.HitInfo.normal);

                    // call collision events
                    var args = new CollisionEventArgs(firstCollision.HitInfo);
                    OnCollide.Invoke(args);
                    firstCollision.CollidedOrb.OnCollisionEnter.Invoke(args);
                }
                else
                {
                    // move all the way up
                    transform.position += remainingMovement;
                    remainingMovement = Vector3.zero;
                }
            }

            // jump backwards the distance the object will move since the above is all calculated relatively
            transform.position -= movement;
        }

        #region Collision
        private void checkForTriggers(CollisionEventArgs args)
        {
            CollisionOrbTrigger orbTrigger = args.HitInfo.collider.GetComponent<CollisionOrbTrigger>();
            if (orbTrigger != null)
            {
                orbTrigger.OnActivated.Invoke(args);
            }
        }
        #endregion

        #region Helper Functions
        private void ApplyRecording(CollisionGroupPositionRecording recording)
        {
            transform.position = recording.ParentPosition;
            SwivelTransform.rotation = recording.ParentRotation;
        }


        private bool TestForCollision(CollisionGroupPositionRecording start, CollisionGroupPositionRecording end, int ignoreThisIndex = -1)
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

        private TestResult FindFirstCollision(CollisionGroupPositionRecording start, CollisionGroupPositionRecording end)
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

        private Vector3 GetRealCollisionPoint(Quaternion addRotation, CollisionGroupPositionRecording start, TestResult firstCollision, int pivotIndex)
        {
            // we need to look for the intersection point of a sphere and a plane
            // that's the same as the intersection of a point and a plane
            // ...if the plane is shifted forward by 1 radius
            Vector3 planeOrigin = firstCollision.HitInfo.point + firstCollision.HitInfo.normal * (CollisionOrbs[pivotIndex].Radius);

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
                Vector3 intersectPoint = rotSystem.Compose(localRotIntersect);

                if (float.IsNaN(closestPoint.x) || intersectPoint.sqrMagnitude < closestPoint.sqrMagnitude)
                {
                    closestPoint = intersectPoint;
                }
            }

            return closestPoint + start.ParentPosition;
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

        private class LayerChangeScope : IDisposable
        {
            private GameObject Parent;
            private int OriginalLayer;

            public LayerChangeScope(GameObject parent, int newLayer)
            {
                Parent = parent;
                OriginalLayer = parent.layer;
                parent.gameObject.layer = newLayer;
            }

            public void Dispose()
            {
                Parent.layer = OriginalLayer;
            }
        }
    }
}
