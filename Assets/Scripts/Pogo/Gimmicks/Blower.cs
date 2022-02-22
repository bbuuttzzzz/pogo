using UnityEngine;

namespace Pogo
{
    public class Blower : MonoBehaviour
    {
        public Vector3 LocalDirection = Vector3.forward;

        public float WindSpeed;
        public float Drag = 1;

        Collider cachedPlayerCollider;
        PlayerController cachedPlayer;
        public void Blow(Collider other)
        {
            if (cachedPlayerCollider != other)
            {
                // LAZY AS FUCK!!!! lol
                PlayerController player = other.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    cachedPlayer = player;
                }
                else
                {
                    return;
                }
            }

            Vector3 direction = transform.TransformVector(LocalDirection);
            float deltaSpeed = WindSpeed - Vector3.Dot(cachedPlayer.Velocity, direction);
            if (deltaSpeed < 0) return;

            cachedPlayer.ApplyForce(direction * Drag * deltaSpeed * deltaSpeed * Time.deltaTime * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            Mesh arrowMesh = Resources.Load<Mesh>("Models/axisIndicator");

            Quaternion localAxisRotation = Quaternion.LookRotation(LocalDirection, Vector3.up);
            Gizmos.color = Color.white;
            Gizmos.DrawMesh(arrowMesh, transform.position, transform.rotation * localAxisRotation, Vector3.one);
        }
    }
}
