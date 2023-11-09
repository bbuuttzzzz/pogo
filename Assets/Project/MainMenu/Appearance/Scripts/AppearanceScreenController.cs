using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Cosmetics
{
    public class AppearanceScreenController : MonoBehaviour
    {
        private enum ScreenIds
        {
            MainScreen
        }

        public PogoMainMenuController parent;

        public GameObject MainScreenRoot;

        private ScreenIds CurrentScreen;

        private const int ScreenCount = 1;
        [NonSerialized]
        private GameObject[] ScreenRoots;

        private void Awake()
        {
            ScreenRoots = new GameObject[ScreenCount];
            ScreenRoots[(int)ScreenIds.MainScreen] = MainScreenRoot;
        }

        public void Return()
        {
            switch (CurrentScreen)
            {
                case ScreenIds.MainScreen:
                    parent.OpenHomeScreen();
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
    }
}