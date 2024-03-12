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
        public enum ScreenIds
        {
            MapSelect,
            UploadSelect,
            UploadDialog,
        }

        public PogoMainMenuController parent;
        public CustomMapSelectScreen MapSelectScreen;
        public UploadDialogScreen UploadScreen;
        private PogoGameManager gameManager;
        [HideInInspector]
        public ScreenIds? OverrideOpenMapScreen;

        public MapHeader CurrentMap;

        [Tooltip("Just read the code for this bro")]
        public GameObject[] ScreenRoots;
        private ScreenIds CurrentScreen;
        private int ScreenCount => ScreenRoots.Length;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;


            if (gameManager.PlatformService.SupportsWorkshop)
            {
                MapSelectScreen.OnUploadPressed.AddListener(() => OpenScreen(ScreenIds.UploadDialog));
            }
        }

        private void OnEnable()
        {
            OpenScreen(OverrideOpenMapScreen.GetValueOrDefault(ScreenIds.MapSelect));
            OverrideOpenMapScreen = null;
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