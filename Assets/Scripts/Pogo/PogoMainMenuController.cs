using Pogo.Challenges;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public UnityEvent OnOpenGamemodeScreen;
        public UnityEvent OnOpenHomeScreen;
        public UnityEvent OnOpenChallengeScreen;
        public UnityEvent OnOpenCustomChallengeScreen;
        public UnityEvent OnHideCustomChallengeScreen;

        public EquipmentSelectorController PogoSelector;
        public PogoChapterSelectorController ChapterSelector;

        public void SetGamemodeOrStart()
        {
            if (PogoSelector.UnlockedEquipment.Length <= 1
                && ChapterSelector.UnlockedChapters.Length <= 1)
            {
                StartGame();
            }
            else
            {
                OpenGamemodeScreen();
            }
        }

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

        public void OpenGamemodeScreen()
        {
            OnOpenGamemodeScreen?.Invoke();
        }

        public void OpenHomeScreen()
        {
            OnOpenHomeScreen?.Invoke();
        }

        public void OpenChallengeScreen()
        {
            OnOpenChallengeScreen?.Invoke();
        }

        public void OpenCustomChallengeScreen()
        {
            OnOpenCustomChallengeScreen?.Invoke();
        }

        public void HideCustomChallengeScreen()
        {
            OnHideCustomChallengeScreen?.Invoke();
        }

        public void StartGame()
        {
            ChapterSelector.LoadActiveChapter();
        }

        public InputField ChallengeInputField;
        public void SelectChallengeTextbox()
        {
            ChallengeInputField.Select();
            ChallengeInputField.ActivateInputField();
        }
    }
}
