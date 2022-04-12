using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class DifficultyListener : MonoBehaviour
    {
        public UnityEvent OnNormalMode;
        public UnityEvent OnHardMode;
        public UnityEvent OnExpertMode;
        public UnityEvent OnFreeplayMode;

        private void Start()
        {
            if (PogoGameManager.GameInstanceIsValid())
            {
                var difficulty = PogoGameManager.PogoInstance.CurrentDifficulty;

                if (difficulty == PogoGameManager.Difficulty.Normal)
                {
                    OnNormalMode?.Invoke();
                }    
                else if (difficulty == PogoGameManager.Difficulty.Hard)
                {
                    OnHardMode?.Invoke();
                }
                else if (difficulty == PogoGameManager.Difficulty.Freeplay)
                {
                    OnFreeplayMode?.Invoke();
                }
                else if (difficulty == PogoGameManager.Difficulty.Expert)
                {
                    OnExpertMode?.Invoke();
                }
            }
            
        }
    }
}
