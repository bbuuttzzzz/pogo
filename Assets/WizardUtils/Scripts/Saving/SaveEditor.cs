using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils.Saving
{
    public class SaveEditor : MonoBehaviour
    {
        public SaveValueDescriptor Save;

        public UnityEvent OnUnlocked;

        public void SetUnlocked(bool newValue)
        {
            if (newValue == Save.IsUnlocked)
            {
                return;
            }
            Save.IsUnlocked = newValue;

            if (newValue)
            {
                OnUnlocked?.Invoke();
            }

            GameManager.GameInstance?.SaveData();
        }
    }
}
