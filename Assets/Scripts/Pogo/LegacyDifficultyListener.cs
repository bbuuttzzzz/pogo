using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class LegacyDifficultyListener : MonoBehaviour
    {
        public UnityEvent OnNormalMode;
        public UnityEvent OnHardMode;
        public UnityEvent OnExpertMode;
        public UnityEvent OnFreeplayMode;
        public UnityEvent OnChallengeMode;

        private void Start()
        {
            if (PogoGameManager.GameInstanceIsValid())
            {
                var difficulty = PogoGameManager.PogoInstance.CurrentDifficulty;
                switch (difficulty)
                {
                    case PogoGameManager.Difficulty.Normal:
                        OnNormalMode?.Invoke();
                        break;
                    case PogoGameManager.Difficulty.Hard:
                        OnHardMode?.Invoke();
                        break;
                    case PogoGameManager.Difficulty.Freeplay:
                        OnFreeplayMode?.Invoke();
                        break;
                    case PogoGameManager.Difficulty.Expert:
                        OnExpertMode?.Invoke();
                        break;
                    case PogoGameManager.Difficulty.Challenge:
                        OnChallengeMode?.Invoke();
                        break;
                }
            }
            
        }
    }
}
