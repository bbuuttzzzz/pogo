using Pogo.Challenges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo
{
    public class ChallengePackMenuController : MonoBehaviour
    {
        public TextMeshProUGUI PackTitleText;
        public Button IncrementButton;
        public Button DecrementButton;
        public ChallengeButtonController[] ChallengeButtons;

        public ChallengePackDescriptor[] Packs;
        [HideInInspector]
        public ChallengePackDescriptor[] UnlockedPacks;
        private ChallengePackDescriptor CurrentPack => UnlockedPacks[ActivePackIndex];
        private int ActivePackIndex = 0;

        public UnityEvent OnActivePackChanged;

        private void Start()
        {
            SetupUnlockedPacks();

            OnActivePackChanged?.AddListener(onPackChanged);
            OnActivePackChanged?.Invoke();

            if (IncrementButton != null) IncrementButton.onClick.AddListener(Increment);
            if (DecrementButton != null) DecrementButton.onClick.AddListener(Decrement);
        }

        private void onPackChanged()
        {
            updateButtonInteractableStates();
            updateChapterName();
            updateChallengeButtons();
        }

        private void updateChallengeButtons()
        {
            for (int n = 0; n < ChallengeButtons.Length; n++)
            {
                if (n >= CurrentPack.Challenges.Length)
                {
                    ChallengeButtons[n].DeveloperChallenge = null;
                }
                else
                {
                    ChallengeButtons[n].DeveloperChallenge = CurrentPack.Challenges[n];
                }
            }
        }

        private void updateChapterName()
        {
            PackTitleText.text = CurrentPack.PrintName;
        }

        private void SetupUnlockedPacks()
        {
            List<ChallengePackDescriptor> unlockedPackList = new List<ChallengePackDescriptor>();
            foreach (ChallengePackDescriptor pack in Packs)
            {
                if (pack.IsUnlocked)
                {
                    unlockedPackList.Add(pack);
                }
            }

            UnlockedPacks = unlockedPackList.ToArray();
        }
        public void Increment()
        {
            ActivePackIndex = Math.Min(UnlockedPacks.Length - 1, ActivePackIndex + 1);
            OnActivePackChanged?.Invoke();
        }
        public void Decrement()
        {
            ActivePackIndex = Math.Max(0, ActivePackIndex - 1);
            OnActivePackChanged?.Invoke();
        }

        private void updateButtonInteractableStates()
        {
            DecrementButton.interactable = (ActivePackIndex > 0);
            IncrementButton.interactable = (ActivePackIndex < UnlockedPacks.Length - 1);
        }
    }
}
