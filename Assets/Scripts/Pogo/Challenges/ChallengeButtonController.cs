using Pogo.MainMenu;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo.Challenges
{
    public class ChallengeButtonController : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI CreatorText;
        public TextMeshProUGUI TimerText;
        public Image MedalImage;

        public Sprite NoMedalSprite;
        public Sprite SilverMedalSprite;
        public Sprite GoldMedalSprite;

        public PogoMainMenuController MainMenuController;

        [SerializeField]
        DeveloperChallenge developerChallenge;
        public DeveloperChallenge DeveloperChallenge
        {
            get => developerChallenge;
            set
            {
                developerChallenge = value;
                OnChallengeChanged();
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(onButtonPressed);
            OnChallengeChanged();
        }

        private void onButtonPressed()
        {
            MainMenuController.LoadDeveloperChallenge(DeveloperChallenge);
        }

        [ContextMenu("Challenge Changed")]
        public void OnChallengeChanged()
        {
            if (DeveloperChallenge == null)
            {
                TitleText.text = "";
                CreatorText.text = "";
                TimerText.text = "";
                MedalImage.sprite = NoMedalSprite;
            }
            else
            {
                SetHasCreator(!string.IsNullOrEmpty(DeveloperChallenge.CreatorName));

                int bestTimeMS = DeveloperChallenge.BestTimeMS;
                float bestTime = (float)bestTimeMS / 1000;
                DeveloperChallenge.Challenge.PersonalBestTimeMS = (ushort)bestTimeMS;

                TitleText.text = DeveloperChallenge.DisplayName;
                CreatorText.text = DeveloperChallenge.CreatorName;
                if (bestTimeMS < DeveloperChallenge.Challenge.BestTimeMS)
                {
                    MedalImage.sprite = GoldMedalSprite;
                    TimerText.text = bestTime.ToString("N3") + "s";
                }
                else if (bestTimeMS < Challenges.Challenge.WORST_TIME)
                {
                    MedalImage.sprite = SilverMedalSprite;
                    TimerText.text = bestTime.ToString("N3") + "s";
                }
                else
                {
                    MedalImage.sprite = NoMedalSprite;
                    TimerText.text = "--.---s";
                }
            }
            
        }

        private void SetHasCreator(bool hasCreator)
        {
            CreatorText.alpha = hasCreator ? 1 : 0;
            TitleText.verticalAlignment = hasCreator ? VerticalAlignmentOptions.Top : VerticalAlignmentOptions.Middle;
        }
    }
}