using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

namespace Pogo
{
    public class ChapterUnlocker : MonoBehaviour
    {
        public ChapterDescriptor Chapter;

        public UnityEvent OnUnlocked;

        public void SetUnlocked(bool newValue)
        {
            if (newValue == Chapter.IsUnlocked)
            {
                return;
            }
            Chapter.IsUnlocked = newValue;

            if (newValue)
            {
                OnUnlocked?.Invoke();
            }

            GameManager.GameInstance?.SaveData();
        }
    }
}
