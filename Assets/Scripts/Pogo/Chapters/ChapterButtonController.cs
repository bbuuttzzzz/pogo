using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo.Challenges
{
    public class ChapterButtonController : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI TitleText;
        public Image IconImage;

        public Sprite NotIncludedSprite;
        public Sprite LockedSprite;
        public PogoMainMenuController MainMenuController;

        [SerializeField]
        ChapterDescriptor chapter;
        public ChapterDescriptor Chapter
        {
            get => chapter;
            set
            {
                chapter = value;
                OnChapterChanged();
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(onButtonPressed);
            OnChapterChanged();
        }

        private void onButtonPressed()
        {
            MainMenuController.LoadChapter(Chapter);
        }

        [ContextMenu("Chapter Changed")]
        public void OnChapterChanged()
        {
            if (Chapter == null)
            {
                TitleText.text = "Steam Version Only";
                IconImage.sprite = NotIncludedSprite;
            }
            else if (!Chapter.IsUnlocked)
            {
                TitleText.text = "Locked";
                IconImage.sprite = LockedSprite;
            }
            else
            {
                TitleText.text = Chapter.Title;
                IconImage.sprite = Chapter.Icon;
            }
        }
    }
}