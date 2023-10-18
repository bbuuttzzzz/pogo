using UnityEngine;

namespace Pogo
{
    public class SlowFaller : MonoBehaviour
    {
        public float MaxFallSpeed;
        public float Drag = 1;

        Collider cachedPlayerCollider;
        PlayerController cachedPlayer;

        private ContinuousForce Force;

        private void Awake()
        {
            Force = new ContinuousForce(this);
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

        public class ContinuousForce : IPlayerContinuousForce
        {
            private SlowFaller parent;

            public ContinuousForce(SlowFaller parent)
            {
                this.parent = parent;
            }

            public bool IsValid() => parent != null;

            public Vector3 GetForce(PlayerController player, float deltaTime)
            {
                float deltaSpeed = parent.MaxFallSpeed - Vector3.Dot(player.Velocity, Vector3.down);
                if (deltaSpeed > 0) return Vector3.zero;

                return Vector3.down * parent.Drag * deltaSpeed * deltaTime;
            }
        }
    }
}
