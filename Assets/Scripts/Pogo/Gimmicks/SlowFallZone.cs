using UnityEngine;

namespace Pogo
{
    public class Slo : MonoBehaviour
    {
        public float MaxFallSpeed;
        public float Drag = 1;

        Collider cachedPlayerCollider;
        PlayerController cachedPlayer;
        public void Slow(Collider other)
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

            float deltaSpeed = MaxFallSpeed - Vector3.Dot(cachedPlayer.Velocity, Vector3.down);
            if (deltaSpeed > 0) return;

            cachedPlayer.ApplyForce(Vector3.up * Drag * deltaSpeed * deltaSpeed * Time.deltaTime * Time.deltaTime);
        }
    }
}
