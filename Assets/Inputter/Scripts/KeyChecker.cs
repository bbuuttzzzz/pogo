using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Inputter
{
    public class KeyChecker : MonoBehaviour
    {
        public KeyName CheckKey;

        public UnityEvent OnKeyHeld;
        public UnityEvent OnKeyPressed;
        public UnityEvent OnKeyReleased;

        private void Update()
        {
            if (InputManager.CheckKey(CheckKey))
            {
                OnKeyHeld?.Invoke();
            }
            if (InputManager.CheckKeyDown(CheckKey))
            {
                OnKeyPressed?.Invoke();
            }
            if (InputManager.CheckKeyUp(CheckKey))
            {
                OnKeyReleased?.Invoke();
            }
        }
    }
}