using Pogo.Challenges;
using Pogo.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;
using WizardUtils.Saving;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public PogoChapterSelectorController ChapterSelector;

        public Animator MainMenuAnimator;
        public Animator ChallengeButtonAnimator;

        private void Start()
        {
            UnlockChecker unlockChecker = GetComponent<UnlockChecker>();
            bool value = unlockChecker.Check();
            ChallengeButtonAnimator.SetBool("Flash", value);
        }

        public void AdventureTapped()
        {
            OpenSavesScreen();
        }


        #region Chapter Loading
        private ChapterDescriptor SelectedChapter;

        public void LoadChapter(ChapterDescriptor chapter)
        {
            SelectedChapter = chapter;
            LoadSelectedChapter();
        }

        public void LoadSelectedChapter()
        {
            PogoGameManager.PogoInstance.LoadChapter(SelectedChapter);
        }

        public void LoadQuickSave()
        {
            PogoGameManager.PogoInstance.LoadQuickSave();
        }

        #endregion

        #region Challenge Loading
        public string DefaultCode;
        public string CurrentCode { get; set; }
        public void LoadChallenge()
        {
            var builder = PogoGameManager.PogoInstance.ChallengeBuilder;
            builder.SetCode(CurrentCode == null || CurrentCode == "" ? DefaultCode : CurrentCode, true);
            builder.DecodeAndLoadCurrentCode();
        }

        public void LoadDeveloperChallenge(DeveloperChallenge developerChallenge)
        {
            var builder = PogoGameManager.PogoInstance.ChallengeBuilder;
            var clonedChallenge = new Challenge()
            {
                LevelState = developerChallenge.Challenge.LevelState,
                BestTimeMS = developerChallenge.Challenge.BestTimeMS,
                PersonalBestTimeMS = (ushort)developerChallenge.BestTimeMS,
                ShareStartPointCm = developerChallenge.Challenge.ShareStartPointCm,
                ShareEndPointCm = developerChallenge.Challenge.ShareEndPointCm,
                StartYaw = developerChallenge.Challenge.StartYaw,
                DeveloperChallenge = developerChallenge,
                ChallengeType = Challenge.ChallengeTypes.PlayDeveloper
            };
            builder.CurrentChallenge = clonedChallenge;
            builder.LoadChallenge();
        }
        #endregion

        #region Options
        [System.Serializable]
        public enum OptionsMenus
        {
            Disambiguation = 0,
            GameOptions = 1,
            SoundOptions = 2,
            VideoOptions = 3
        }

        public GameObject MainMenuOccluder;
        public ToggleableUIElement[] OptionsScreens;

        public void OpenOptionsScreen_Disambiguation() => OpenOptionsScreen(OptionsMenus.Disambiguation);
        public void OpenOptionsScreen_GameOptions() => OpenOptionsScreen(OptionsMenus.GameOptions);
        public void OpenOptionsScreen_SoundOptions() => OpenOptionsScreen(OptionsMenus.SoundOptions);
        public void OpenOptionsScreen_VideoOptions() => OpenOptionsScreen(OptionsMenus.VideoOptions);

        public void OpenOptionsScreen(OptionsMenus optionsMenu)
        {
            for(int n = 0; n < OptionsScreens.Length; n++)
            {
                OptionsScreens[n].SetOpen((int)optionsMenu == n);
            }
            MainMenuOccluder.SetActive(true);
        }

        public void CloseOptionsScreen()
        {
            for (int n = 0; n < OptionsScreens.Length; n++)
            {
                OptionsScreens[n].SetOpen(false);
            }
            MainMenuOccluder.SetActive(false);
        }
        #endregion

        public void OpenWorldScreen()
        {
            MainMenuAnimator.SetTrigger("ShowWorld");
        }

        public void OpenSavesScreen()
        {
            MainMenuAnimator.SetTrigger("ShowSaves");
        }

        public void OpenCustomMapsScreen()
        {
            MainMenuAnimator.SetTrigger("ShowCustomMaps");
        }

        public void OpenAppearanceScreen()
        {
            MainMenuAnimator.SetTrigger("ShowAppearance");
        }

        public void OpenHomeScreen()
        {
            Animator animator = GetComponent<Animator>();
            UnlockChecker unlockChecker = GetComponent<UnlockChecker>();
            bool value = unlockChecker.Check();
            animator.SetBool("ChallengeModeFlash", value);
            MainMenuAnimator.SetTrigger("ShowHome");
        }

        public void OpenChallengeScreen()
        {
            GetComponent<SaveEditor>().SetUnlocked(false);
            MainMenuAnimator.SetTrigger("ShowChallenge");
        }

        public void OpenCustomChallengeScreen()
        {
            MainMenuAnimator.SetTrigger("ShowCustomChallenge");
        }

        public void HideCustomChallengeScreen()
        {
            MainMenuAnimator.SetTrigger("HideCustomChallenge");
        }

        public void StartGame()
        {
            LoadSelectedChapter();
        }

        public InputField ChallengeInputField;
        public void SelectChallengeTextbox()
        {
            ChallengeInputField.Select();
            ChallengeInputField.ActivateInputField();
        }
    }
}
