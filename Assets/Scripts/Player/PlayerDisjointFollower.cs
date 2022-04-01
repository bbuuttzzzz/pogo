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

#if UNITY_EDITOR
            if (player == null)
            {
                Debug.LogError($"Missing Player for PlayerDisjointFollower {gameObject.name}", gameObject);
                return;
            }
#endif
            player.OnDisjoint?.AddListener(() => OnPlayerDisjoint?.Invoke());
        }
    }
}