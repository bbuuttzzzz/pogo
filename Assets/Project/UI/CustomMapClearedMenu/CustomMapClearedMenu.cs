using System;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapClearedMenu : ToggleableUIElement
    {
        private PogoGameManager gameManager;

        public Button RestartButton;
        public Button QuitButton;

        private void Start()
        {
            gameManager = PogoGameManager.PogoInstance;
            OnOpen.AddListener(Base_OnOpen);
            OnClose.AddListener(Base_OnClose);
            RestartButton.onClick.AddListener(Restart);
            QuitButton.onClick.AddListener(Quit);
        }

        private void Restart()
        {
            gameManager.CustomMapBuilder.RestartMap();
        }

        private void Quit()
        {
            gameManager.Quit(false);
        }


        private void Base_OnOpen()
        {
            gameManager.ForceShowTimer = true;
        }

        private void Base_OnClose()
        {
            gameManager.ForceShowTimer = false;
        }
    }
}
