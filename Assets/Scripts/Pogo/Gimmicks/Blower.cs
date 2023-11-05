using System;
using UnityEngine;

namespace Pogo
{
    public class Blower : MonoBehaviour
    {
        public Vector3 LocalDirection = Vector3.forward;

        public float WindSpeed;
        public float Drag = 1;

        BlowerForce Force;

        private void Awake()
        {
            Force = new BlowerForce(this);
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


        private void OnDrawGizmosSelected()
        {
            Mesh arrowMesh = Resources.Load<Mesh>("Models/axisIndicator");

            Quaternion localAxisRotation = Quaternion.LookRotation(LocalDirection, Vector3.up);
            Gizmos.color = Color.white;
            Gizmos.DrawMesh(arrowMesh, transform.position, transform.rotation * localAxisRotation, Vector3.one);
        }

        public class BlowerForce : IPlayerContinuousForce
        {
            private Blower parent;

            public BlowerForce(Blower parent)
            {
                this.parent = parent;
            }

            public bool IsValid() => parent != null;

            public Vector3 GetForce(PlayerController player, float deltaTime)
            {
                Vector3 direction = parent.transform.TransformVector(parent.LocalDirection);
                float deltaSpeed = parent.WindSpeed - Vector3.Dot(player.Velocity, direction);
                if (deltaSpeed < 0) return Vector3.zero;

                return direction * parent.Drag * deltaSpeed * deltaTime;
            }
        }
    }
}
