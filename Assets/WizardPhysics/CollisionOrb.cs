using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WizardPhysics
{
    public class CollisionOrb : MonoBehaviour
    {
        public bool QueryExits;

        public CollisionEvent OnCollisionEnter;
        public CollisionEvent OnCollisionExit;

        public Vector3 Center;
        public float Radius;

        public bool CheckPassively;

        private void Awake()
        {
            cachedLayerMask = LAYERMASK.MaskForLayer(gameObject.layer);
            Disjoint();
            if (CheckPassively)
            {
                OnCollisionEnter.AddListener(CheckForTriggers);
            }
        }

        public void CheckForTriggers(CollisionEventArgs args)
        {
            CollisionOrbTrigger orbTrigger = args.HitInfo.collider.GetComponent<CollisionOrbTrigger>();
            if (orbTrigger != null)
            {
                orbTrigger.OnActivated.Invoke(args);
            }
        }

        private void Update()
        {
            if (CheckPassively) CheckThisFramesMovement();
        }

        Vector3 lastRecordedPosition;
        void CheckThisFramesMovement()
        {
            if (TestPath(lastRecordedPosition, currentCenter, out RaycastHit hitinfo))
            {
                OnCollisionEnter?.Invoke(new CollisionEventArgs(hitinfo));
            }

            if (QueryExits && TestPath(currentCenter, lastRecordedPosition, out RaycastHit exitInfo))
            {
                OnCollisionExit?.Invoke(new CollisionEventArgs(exitInfo));
            }

            lastRecordedPosition = currentCenter;
        }

        public bool TestPath(Vector3 start, Vector3 finish, out RaycastHit hitInfo)
        {
            Vector3 direction = finish - start;
            float maxDistance = (finish - start).magnitude;
            direction /= maxDistance;
            return Physics.SphereCast(
                ray: new Ray(start, direction),
                radius: Radius * transform.lossyScale.x,
                maxDistance: maxDistance,
                hitInfo: out hitInfo,
                layerMask: cachedLayerMask);
        }

        public bool TestRay(Ray ray, float maxDistance, out RaycastHit hitInfo)
        {
            return TestPath(ray.origin, ray.origin + ray.direction * maxDistance, out hitInfo);
        }

        /// <summary>
        /// Reset last recorded position. you should call this immediately after teleporting
        /// </summary>
        public void Disjoint()
        {
            Debug.Log($"Disjointed {name}");
            lastRecordedPosition = currentCenter;
        }

        Vector3 currentCenter => transform.TransformPoint(Center);

        int cachedLayerMask;

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.TransformPoint(Center), transform.lossyScale.x * Radius);
        }
        #endregion

        public struct CollisionResult
        {
            public bool DidHit;
            public RaycastHit HitInfo;

            public static CollisionResult noHit = new CollisionResult() { DidHit = false };
            public static CollisionResult Hit(RaycastHit hitInfo)
            {
                return new CollisionResult() { DidHit = false, HitInfo = hitInfo };
            }
        }
    }
}
