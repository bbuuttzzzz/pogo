using System;
using UnityEngine;

namespace WizardUI
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
