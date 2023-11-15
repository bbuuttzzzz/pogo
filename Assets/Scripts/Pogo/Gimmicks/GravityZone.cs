using System;
using UnityEngine;

namespace Pogo
{
    public class GravityZone : MonoBehaviour
    {
        public Vector3 DeltaGravity;

        GravityForce Force;

        private void Awake()
        {
            Force = new GravityForce(this);
        }

        public void Apply(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.AddContinuousForce(Force);
            }
            else
            {
                return;
            }
        }

        public void Remove(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.RemoveContinuousForce(Force);
            }
            else
            {
                return;
            }
        }

        public class GravityForce : IPlayerContinuousForce
        {
            private GravityZone parent;

            public GravityForce(GravityZone parent)
            {
                this.parent = parent;
            }

            public bool IsValid() => parent != null;

            public Vector3 GetForce(PlayerController player, float deltaTime)
            {
                return parent.DeltaGravity * deltaTime;
            }
        }
    }
}
