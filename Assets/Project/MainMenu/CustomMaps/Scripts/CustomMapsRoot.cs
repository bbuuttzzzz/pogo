using Pogo.CustomMaps.Indexing;
using Pogo.MainMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WizardUI;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapsRoot : MonoBehaviour
    {
        private enum ScreenIds
        {
            MapSelect,
            UploadSelect,
            UploadDialog,
        }

        public PogoMainMenuController parent;
        private PogoGameManager gameManager;
        public Button UploadButton;

        public MapHeader CurrentMap;

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
                UploadButton.onClick.AddListener(() => OpenScreen(ScreenIds.UploadSelect));
            }
        }

        public void OpenUploadDialog(MapHeader selectedMap)
        {
            CurrentMap = selectedMap;
            OpenScreen(ScreenIds.UploadDialog);
        }

        #region Menu Navigation

        public void Back()
        {
            switch (CurrentScreen)
            {
                case ScreenIds.MapSelect:
                    parent.OpenHomeScreen();
                    break;
                case ScreenIds.UploadSelect:
                    OpenScreen(ScreenIds.MapSelect);
                    break;
                case ScreenIds.UploadDialog:
                    OpenScreen(ScreenIds.UploadSelect);
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