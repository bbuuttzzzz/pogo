using System;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils;

namespace WizardPhysics
{
    [RequireComponent(typeof(Collider))]
    public class OrbSafeMover : MonoBehaviour
    {
        private List<CollisionGroup> Subscribers = new List<CollisionGroup>();
        Collider self;
        
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

        public void MoveTo(Vector3 finalPosition)
        {
            if (transform.position == finalPosition) { return; }

            foreach(var collisionGroup in Subscribers)
            {
                collisionGroup.TestAgainst(self, finalPosition - transform.position);
            }

            transform.position = finalPosition;
        }
    }
}
