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

    }
}
