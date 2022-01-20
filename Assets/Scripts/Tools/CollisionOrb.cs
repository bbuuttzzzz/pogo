using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Collision
{
    public class CollisionOrb : MonoBehaviour
    {
        public bool QueryExits;

        public CollisionEvent OnCollisionEnter;
        public CollisionEvent OnCollisionExit;

        public Vector3 Center;
        public float Radius;


        private void Awake()
        {
            cachedLayerMask = LAYERMASK.MaskForLayer(gameObject.layer);
            Disjoint();
        }

        private void Update()
        {
            Move();
        }

        Vector3 lastRecordedPosition;
        void Move()
        {
            if (CheckPath(lastRecordedPosition, currentCenter, out RaycastHit hitinfo))
            {
                OnCollisionEnter?.Invoke(new CollisionEventArgs(hitinfo));
            }

            if (QueryExits && CheckPath(currentCenter, lastRecordedPosition, out RaycastHit exitInfo))
            {
                OnCollisionExit?.Invoke(new CollisionEventArgs(exitInfo));
            }

            lastRecordedPosition = currentCenter;
        }

        bool CheckPath(Vector3 start, Vector3 finish, out RaycastHit hitInfo)
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

        /// <summary>
        /// Reset last recorded position. you should call this immediately after teleporting
        /// </summary>
        public void Disjoint()
        {
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
    }
}
