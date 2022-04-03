using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class DifficultyListener : MonoBehaviour
    {
        public UnityEvent OnHardMode;
        public UnityEvent OnHardcoreMode;

        private void Start()
        {
            if (PogoGameManager.GameInstanceIsValid())
            {
                var difficulty = PogoGameManager.PogoInstance.CurrentDifficulty;

                if (difficulty == PogoGameManager.Difficulty.Hard)
                {
                    OnHardMode?.Invoke();
                }
                else if (difficulty == PogoGameManager.Difficulty.Hardcore)
                {
                    OnHardcoreMode?.Invoke();
                }
            }
            
        }
    }
}
