using Pogo.Saving;
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

        public Sprite SteamOnlySprite;
        public Sprite ComingSoonSprite;
        public Sprite LockedSprite;
        public PogoMainMenuController MainMenuController;

        [SerializeField]
        WorldChapter worldChapter;
        public WorldChapter WorldChapter
        {
            get => worldChapter;
            set
            {
                worldChapter = value;
                OnChapterChanged();
            }
        }
        private ChapterSaveData saveData;


        private void Awake()
        {
            button.onClick.AddListener(onButtonPressed);
            OnChapterChanged();
        }

        private void onButtonPressed()
        {
            MainMenuController.SelectChapter(WorldChapter.Chapter);
        }

        [ContextMenu("Chapter Changed")]
        public void OnChapterChanged()
        {
            switch (WorldChapter.Type)
            {
                case WorldChapter.Types.Level:
                    LoadSaveData();
                    UpdateLevelSelectDisplay();
                    break;
                case WorldChapter.Types.ComingSoon:
                    TitleText.text = "Coming Soon";
                    IconImage.sprite = ComingSoonSprite;
                    break;
                case WorldChapter.Types.SteamOnly:
                    TitleText.text = "Steam Version Only";
                    IconImage.sprite = ComingSoonSprite;
                    break;
            }
        }

        private void LoadSaveData()
        {
            saveData = PogoGameManager.PogoInstance.GetChapterSaveData(WorldChapter.Chapter);
        }

        private void UpdateLevelSelectDisplay()
        {
#if DEBUG
            if (WorldChapter.Chapter == null)
            {
                Debug.LogError($"Missing Chapter for WorldChapter @ {gameObject.name}");
            }
#endif

            if (saveData.unlocked)
            {
                TitleText.text = WorldChapter.Chapter.Title;
                IconImage.sprite = WorldChapter.Chapter.Icon;
            }
            else
            {
                TitleText.text = "Locked";
                IconImage.sprite = LockedSprite;
            }
        }
    }
}