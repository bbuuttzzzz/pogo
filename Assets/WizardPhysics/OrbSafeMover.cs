using System;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils;

namespace WizardPhysics
{
    [RequireComponent(typeof(Collider))]
    public class OrbSafeMover : MonoBehaviour, ISpecialPlayerCollisionBehavior
    {
        private List<CollisionGroup> Subscribers = new List<CollisionGroup>();
        Collider self;
        private Vector3 lastVelocity;
        public bool ShouldFollowTargetPosition;
        public bool TargetPositionIsLocal;
        public Vector3 TargetPosition;

        [System.Serializable]
        public enum PlayerCollisionBehavior
        {
            AlwaysInheritVelocity,
            NeverInheritVelocity
        }
        public PlayerCollisionBehavior CollisionBehavior;

        private void Awake()
        {
            self = GetComponent<Collider>();    
        }

        private void Update()
        {
            if (ShouldFollowTargetPosition)
            {
                FollowTargetPosition();
            }
        }

        public void FollowTargetPosition()
        {
            Vector3 targetPosition = TargetPositionIsLocal ? transform.parent.TransformPoint(TargetPosition) : TargetPosition;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                transform.position = targetPosition;
                return;
            }
#endif

            MoveTo(targetPosition);
        }

        public void Subscribe(CollisionGroup group)
        {
            Subscribers.Add(group);
        }

        public void Unsubscribe(CollisionGroup group)
        {
            Subscribers.Remove(group);
        }

        public void MoveTo(Vector3 finalPosition) => MoveTo(finalPosition, -1);

        public void MoveTo(Vector3 finalPosition, float interval)
        {
            if (interval <= 0) interval = Time.deltaTime;

            lastVelocity = (finalPosition - transform.position) / interval;

            if (transform.position == finalPosition) { return; }

            foreach(var collisionGroup in Subscribers)
            {
                collisionGroup.TestAgainst(self, finalPosition - transform.position);
            }

            transform.position = finalPosition;
        }

        void ISpecialPlayerCollisionBehavior.Perform(PlayerController target, CollisionEventArgs args)
        {
            switch(CollisionBehavior)
            {
                case PlayerCollisionBehavior.AlwaysInheritVelocity:
                    target.Velocity += lastVelocity;
                    break;
                case PlayerCollisionBehavior.NeverInheritVelocity:
                    // noop
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}
