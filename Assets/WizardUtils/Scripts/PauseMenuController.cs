using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WizardUtils
{
    public class PauseMenuController : ToggleableUIElement
    {
        public UnityEvent OnMenuClosed;

        public ToggleableUIElement BaseMenu;

        ToggleableUIElement currentMenu;
        public ToggleableUIElement CurrentMenu
        {
            get
            {
                return currentMenu;
            }
            set
            {
                if (currentMenu == value) return;

                if (currentMenu != null)
                {
                    currentMenu.SetOpen(false);
                }
                if (value != null)
                {
                    value.SetOpen(true);
                }

                currentMenu = value;
            }
        }

        public ToggleableUIElement[] SubMenus;

        public void OpenSubMenu(int index)
        {
            CurrentMenu = SubMenus[index];
        }

        public void ReturnToBaseMenu()
        {
            CurrentMenu = BaseMenu;
        }

        protected virtual void Start()
        {
            currentMenu = null;
            GameManager.GameInstance.OnPauseStateChanged += onPauseStateChanged;
        }

        protected virtual void onPauseStateChanged(object sender, bool nowPaused)
        {
            OnMenuClosed?.Invoke();
            Root?.SetActive(nowPaused);
            CurrentMenu = nowPaused ? BaseMenu : null;
        }

        public void Resume()
        {
            GameManager.Paused = false;
        }

        public void ReturnToMainMenu()
        {
            Resume();
            GameManager.GameInstance?.Quit(false);
        }

    }

}
