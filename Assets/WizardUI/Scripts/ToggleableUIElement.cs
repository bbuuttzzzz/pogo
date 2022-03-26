using System;
using UnityEngine;

namespace WizardUtils
{
    public class ToggleableUIElement : MonoBehaviour
    {
        public GameObject Root;

        public void SetOpen(bool isOpen)
        {
            Root.SetActive(isOpen);
        }

        public bool IsOpen => Root?.activeSelf??false;
    }
}
