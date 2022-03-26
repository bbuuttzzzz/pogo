using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public ChapterDescriptor[] Chapters;
        [HideInInspector]
        public ChapterDescriptor[] UnlockedChapters;
        [HideInInspector]
        public int ActiveChapterIndex = 0;

        public UnityEvent OnActiveChapterChanged;
        public UnityStringEvent OnChapterNameChanged;

        private void Start()
        {
            SetupUnlockedChapters();

            OnActiveChapterChanged.AddListener(updateChapterName);
            OnActiveChapterChanged.AddListener(updateButtonInteractableStates);
            OnActiveChapterChanged?.Invoke();
        }

        private void SetupUnlockedChapters()
        {
            List<ChapterDescriptor> unlockedChapterList = new List<ChapterDescriptor>();
            foreach(ChapterDescriptor chapter in Chapters)
            {
                if (chapter.IsUnlocked)
                {
                    unlockedChapterList.Add(chapter);
                }
            }

            UnlockedChapters = unlockedChapterList.ToArray();
        }

        void updateChapterName()
        {
            OnChapterNameChanged.Invoke(UnlockedChapters[ActiveChapterIndex].LongTitle);
        }

        public void LoadActiveChapter()
        {
            PogoGameManager.PogoInstance.LoadChapter(UnlockedChapters[ActiveChapterIndex]);
        }

        public Button IncrementButton; 
        public void IncrementChapter()
        {
            ActiveChapterIndex = Math.Min(UnlockedChapters.Length - 1, ActiveChapterIndex + 1);
            OnActiveChapterChanged?.Invoke();
        }

        public Button DecrementButton;
        public void DecrementChapter()
        {
            ActiveChapterIndex = Math.Max(0, ActiveChapterIndex - 1);
            OnActiveChapterChanged?.Invoke();
        }

        private void updateButtonInteractableStates()
        {
            DecrementButton.interactable = (ActiveChapterIndex > 0);
            IncrementButton.interactable = (ActiveChapterIndex < UnlockedChapters.Length - 1);
        }
    }
}
