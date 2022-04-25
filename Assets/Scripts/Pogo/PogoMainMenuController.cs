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
            builder.DecodeCurrentCode();
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
