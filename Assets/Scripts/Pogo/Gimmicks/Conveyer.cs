using Pogo.MaterialTypes;
using UnityEngine;
using WizardPhysics;

namespace Pogo.Gimmicks
{
    public class Conveyer : MonoBehaviour, ISpecialPlayerCollisionBehavior
    {
        public Vector3 Direction = Vector3.up;
        public float Speed;
        public bool WorldSpace;

        private Vector3 LocalDirection => WorldSpace ? transform.InverseTransformDirection(Direction)
            : Direction;
        private Vector3 WorldDirection => WorldSpace ? Direction
            : transform.TransformDirection(Direction);

        public bool TryOverrideCollisionBehavior(PlayerController target, CollisionEventArgs args, SurfaceConfig surfaceConfig)
        {
            target.Accelerate(WorldDirection, Speed);
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Mesh arrowMesh = Resources.Load<Mesh>("Models/axisIndicator");

            Quaternion axisRotation = Quaternion.LookRotation(LocalDirection, Vector3.up);

            Gizmos.color = Color.white;
            Gizmos.DrawMesh(arrowMesh, transform.position, transform.rotation * axisRotation, Vector3.one);
        }
    }
}
