using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils
{
    public class ToggleableUIElement : MonoBehaviour
    {
        public GameObject Root;

        public UnityEvent OnOpen;

        public void SetOpen(bool isOpen)
        {
            Root.SetActive(isOpen);
            if (isOpen) OnOpen?.Invoke();
        }

        public bool IsOpen => Root?.activeSelf??false;
    }
}
