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
        
        private void Awake()
        {
            self = GetComponent<Collider>();    
        }

        public void Subscribe(CollisionGroup group)
        {
            Subscribers.Add(group);
        }

        public void Unsubscribe(CollisionGroup group)
        {
            Subscribers.Remove(group);
        }

        public void MoveTo(Vector3 finalPosition, float interval = -1)
        {
            if (interval == -1) interval = Time.deltaTime;
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
            target.Velocity += lastVelocity;
        }
    }
}
