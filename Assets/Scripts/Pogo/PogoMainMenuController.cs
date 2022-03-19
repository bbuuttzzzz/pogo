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
        public ChapterDescriptor[] ChapterList;
        public int ActiveChapterIndex = 0;

        public UnityEvent OnActiveChapterChanged;
        public UnityStringEvent OnChapterNameChanged;

        private void Awake()
        {
            OnActiveChapterChanged.AddListener(updateChapterName);
            OnActiveChapterChanged.AddListener(updateButtonInteractableStates);
            OnActiveChapterChanged?.Invoke();
        }

        void updateChapterName()
        {
            OnChapterNameChanged.Invoke(ChapterList[ActiveChapterIndex].LongTitle);
        }

        public void LoadActiveChapter()
        {
            PogoGameManager.PogoInstance.LoadChapter(ChapterList[ActiveChapterIndex]);
        }

        public Button IncrementButton; 
        public void IncrementChapter()
        {
            ActiveChapterIndex = Math.Min(ChapterList.Length - 1, ActiveChapterIndex + 1);
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
            IncrementButton.interactable = (ActiveChapterIndex < ChapterList.Length - 1);
        }
    }
}
