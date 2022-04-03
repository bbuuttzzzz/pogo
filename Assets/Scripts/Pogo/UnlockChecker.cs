using UnityEngine;
using UnityEngine.Events;
using WizardUtils.Saving;

namespace Pogo
{
    public class UnlockChecker : MonoBehaviour
    {
        public SaveValueDescriptor Save;

        public bool CheckOnAwake = true;

        public UnityEvent OnCheckedLocked;
        public UnityEvent OnCheckedUnlocked;

        public void Start()
        {
            if (CheckOnAwake) Check();
        }

        public bool Check()
        {
            bool unlocked = Save.IsUnlocked;

            if (unlocked)
            {
                OnCheckedUnlocked?.Invoke();
            }
            else
            {
                OnCheckedLocked?.Invoke();
            }

            return unlocked;
        }
    }
}
