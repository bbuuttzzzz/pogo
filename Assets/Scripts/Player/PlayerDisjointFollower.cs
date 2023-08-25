using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class PlayerDisjointFollower : MonoBehaviour
    {
        public UnityEvent OnPlayerDisjoint;

        private void Start()
        {
            var player = GetComponentInParent<PlayerController>(true);

            if (player == null)
            {
                return;
            }
            player.OnDisjoint?.AddListener(() => OnPlayerDisjoint?.Invoke());
        }
    }
}