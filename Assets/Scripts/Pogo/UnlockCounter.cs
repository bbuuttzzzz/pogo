using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils.Saving
{
    public class UnlockCounter : MonoBehaviour
    {
        public SaveValueDescriptor[] Saves;

        public bool CheckOnAwake = true;

        public UnityEvent<int> OnChecked;

        public void Start()
        {
            if (CheckOnAwake) TriggerCheck();
        }

        public void TriggerCheck()
        {
            _ = Check();
        }
        
        public int Check()
        {
            int count = 0;
            foreach(var save in Saves)
            {
                if (save.IsUnlocked) count++;
            }

            return count;
        }
    }
}
