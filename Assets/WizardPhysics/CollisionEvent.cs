using System;
using UnityEngine;
using UnityEngine.Events;

namespace WizardPhysics
{
    [System.Serializable]
    public class CollisionEvent : UnityEvent<CollisionEventArgs>
    {

    }

    public class CollisionEventArgs : EventArgs
    {
        public RaycastHit HitInfo;

        public CollisionEventArgs(RaycastHit hitInfo)
        {
            HitInfo = hitInfo;
        }
    }
}
