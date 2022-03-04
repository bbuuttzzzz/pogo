using System;
using UnityEngine;

namespace WizardUI
{
    public class MenuController : MonoBehaviour
    {
        public GameObject MenuRoot;

        public void SetOpen(bool isOpen)
        {
            MenuRoot.SetActive(isOpen);
        }

        public bool IsOpen => MenuRoot?.activeSelf??false;
    }
}
