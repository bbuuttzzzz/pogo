using Pogo;
using Pogo.Challenges;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo.Cosmetics
{
    public class AppearanceScreenController : MonoBehaviour
    {
        private enum ScreenIds
        {
            MainScreen,
            ChangeCosmeticScreen
        }

        public PogoMainMenuController parent;
        public VendingMachineButtonController VendingMachineButton;
        public VendingUnlockPopupController VendingUnlockPopup;
        public LocalPositionWaypointer VendingMachineWaypointer;
        private VendingMachineUnlockData NextReward;

        [Tooltip("Just read the code for this bro")]
        public GameObject[] ScreenRoots;

        private ScreenIds CurrentScreen;

        private int ScreenCount => ScreenRoots.Length;

        private void Awake()
        {
            VendingMachineButton.GetComponent<Button>().onClick.AddListener(VendingMachineButton_OnClick);
        }

        public void OnEnable()
        {
            OpenScreen(ScreenIds.MainScreen);
            UpdateVendingMachine();
            ShowVendingMachine();
        }

        public void OnDisable()
        {
            PogoGameManager.PogoInstance.SaveGlobalSave();
            HideVendingMachine();
        }

        public void Back()
        {
            switch (CurrentScreen)
            {
                case ScreenIds.MainScreen:
                    parent.OpenHomeScreen();
                    break;
                case ScreenIds.ChangeCosmeticScreen:
                    OpenScreen(ScreenIds.MainScreen);
                    break;
            }
        }

        public void OpenChangeCosmeticPage(CosmeticSlotManifest cosmeticType)
        {
            if (CurrentScreen != ScreenIds.MainScreen) throw new InvalidOperationException();

            OpenScreen(ScreenIds.ChangeCosmeticScreen);
            ScreenRoots[(int)ScreenIds.ChangeCosmeticScreen].GetComponent<ChangeCosmeticScreenController>().Load(cosmeticType);
        }

        private void OpenScreen(ScreenIds id)
        {
            CurrentScreen = id;
            for (int n = 0; n < ScreenCount; n++)
            {
                ScreenRoots[n].SetActive(n == (int)id);
            }

            UpdateVendingMachineButtonCanBeActive();
        }

        private void UpdateVendingMachine()
        {
            VendingMachineButton.QuarterCount = PogoGameManager.PogoInstance.CurrentGlobalDataTracker.SaveData.CollectedCoins;
            if (PogoGameManager.PogoInstance.TryGetNextVendingUnlock(out NextReward))
            {
                VendingMachineButton.NextReward = NextReward;
            }
            else
            {
                VendingMachineButton.NextReward = default;
            }
        }

        private void ShowVendingMachine()
        {
            VendingMachineWaypointer.GoToWaypoint(-1);
        }

        private void HideVendingMachine()
        {
            VendingMachineWaypointer.SnapToWaypoint(0);
        }

        private void VendingMachineButton_OnClick()
        {
            if (PogoGameManager.PogoInstance.TryUnlockNextVendingUnlock(out CosmeticDescriptor unlockedCosmetic))
            {
                VendingMachineButton.TriggerUnlock();
                VendingUnlockPopup.CurrentCosmetic = unlockedCosmetic;
                VendingUnlockPopup.TriggerUnlock();
                UpdateVendingMachine();
            }
        }

        private void UpdateVendingMachineButtonCanBeActive()
        {
            VendingMachineButton.ButtonCanBeActive = CurrentScreen == ScreenIds.MainScreen;
        }
    }
}