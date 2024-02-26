using Pogo;
using Pogo.CustomMaps.Indexing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WizardUI;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapsScreenController : MonoBehaviour
    {
        private enum ScreenIds
        {
            MainScreen,
            UploadSelectScreen
        }

        public PogoMainMenuController parent;
        private PogoGameManager gameManager;
        public Button UploadButton;

        [Tooltip("Just read the code for this bro")]
        public GameObject[] ScreenRoots;
        private ScreenIds CurrentScreen;
        private int ScreenCount => ScreenRoots.Length;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;

            if (!gameManager.PlatformService.SupportsWorkshop)
            {
                UploadButton.gameObject.SetActive(false);
            }
            else
            {
                UploadButton.onClick.AddListener(() => OpenScreen(ScreenIds.UploadSelectScreen));
            }
        }

        #region Menu Navigation

        public void Back()
        {
            switch (CurrentScreen)
            {
                case ScreenIds.MainScreen:
                    parent.OpenHomeScreen();
                    break;
                case ScreenIds.UploadSelectScreen:
                    OpenScreen(ScreenIds.MainScreen);
                    break;
            }
        }

        private void OpenScreen(ScreenIds id)
        {
            CurrentScreen = id;
            for (int n = 0; n < ScreenCount; n++)
            {
                ScreenRoots[n].SetActive(n == (int)id);
            }
        }
        #endregion
    }
}