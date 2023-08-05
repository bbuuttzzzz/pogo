using Pogo.Challenges;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils.Saving;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public EquipmentSelectorController PogoSelector;
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
            OpenWorldScreen();
        }


        #region Chapter Loading
        private ChapterDescriptor SelectedChapter;

        public void SelectChapter(ChapterDescriptor chapter)
        {
            SelectedChapter = chapter;
            OpenGamemodeScreen();
        }

        public void LoadSelectedChapter()
        {
            PogoGameManager.PogoInstance.LoadChapter(SelectedChapter);
        }
        #endregion

        #region Challenge Loading
        public string DefaultCode;
        public string CurrentCode { get; set; }
        public void LoadChallenge()
        {
            var builder = PogoGameManager.PogoInstance.GetComponent<ChallengeBuilder>();
            builder.SetCode(CurrentCode == null || CurrentCode == "" ? DefaultCode : CurrentCode, true);
            builder.DecodeAndLoadCurrentCode();
        }

        public void LoadDeveloperChallenge(DeveloperChallenge developerChallenge)
        {
            var builder = PogoGameManager.PogoInstance.GetComponent<ChallengeBuilder>();
            var clonedChallenge = new Challenge()
            {
                Level = developerChallenge.Challenge.Level,
                BestTimeMS = developerChallenge.Challenge.BestTimeMS,
                PersonalBestTimeMS = (ushort)developerChallenge.BestTimeMS,
                StartPointCm = developerChallenge.Challenge.StartPointCm,
                EndPointCm = developerChallenge.Challenge.EndPointCm,
                StartYaw = developerChallenge.Challenge.StartYaw,
                DeveloperChallenge = developerChallenge,
                ChallengeType = Challenge.ChallengeTypes.PlayDeveloper
            };
            builder.CurrentChallenge = clonedChallenge;
            builder.LoadChallenge();
        }
        #endregion
        public void OpenWorldScreen()
        {
            MainMenuAnimator.SetTrigger("ShowWorld");
        }

        public void OpenGamemodeScreen()
        {
            MainMenuAnimator.SetTrigger("ShowGamemode");
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
