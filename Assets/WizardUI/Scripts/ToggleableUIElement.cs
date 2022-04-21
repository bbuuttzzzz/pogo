using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils
{
    public class ToggleableUIElement : MonoBehaviour
    {
        public GameObject Root;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        public void SetOpen(bool isOpen)
        {
            bool wasOpen = IsOpen;
            Root.SetActive(isOpen);

            if (!wasOpen && isOpen)
            {
                OnOpen?.Invoke();
            }
            else if (wasOpen && !isOpen)
            {
                OnClose?.Invoke();
            }
        }

        public bool IsOpen => Root?.activeSelf??false;
    }
}
