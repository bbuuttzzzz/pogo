using Pogo.Challenges;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo
{
    public class PogoChapterSelectorController : MonoBehaviour
    {
        public WorldDescriptor[] Worlds;
        [HideInInspector]
        public ChapterDescriptor[] DisplayChapters;
        private int ActiveWorldIndex = 0;
        private WorldDescriptor ActiveWorld => Worlds[ActiveWorldIndex];
        public ChapterButtonController[] ChapterButtons;

        public UnityEvent OnActiveWorldChanged;
        public UnityEvent<string> OnWorldNameChanged;

        private void Start()
        {
            OnActiveWorldChanged?.AddListener(OnWorldChanged);
            OnActiveWorldChanged?.Invoke();

            if (IncrementButton != null) IncrementButton.onClick.AddListener(Increment);
            if (DecrementButton != null) DecrementButton.onClick.AddListener(Decrement);
        }

        private void OnWorldChanged()
        {
            UpdateWorldName();
            UpdateButtonInteractableStates();
            UpdateChapterButtons();
        }

        void UpdateWorldName()
        {
            OnWorldNameChanged.Invoke(ActiveWorld.DisplayName);
        }

        public void LoadActiveChapter()
        {
            PogoGameManager.PogoInstance.LoadChapter(DisplayChapters[ActiveWorldIndex]);
        }

        public Button IncrementButton; 
        public void Increment()
        {
            ActiveWorldIndex = Math.Min(DisplayChapters.Length - 1, ActiveWorldIndex + 1);
            OnActiveWorldChanged?.Invoke();
        }

        public Button DecrementButton;
        public void Decrement()
        {
            ActiveWorldIndex = Math.Max(0, ActiveWorldIndex - 1);
            OnActiveWorldChanged?.Invoke();
        }

        private void UpdateButtonInteractableStates()
        {
            DecrementButton.interactable = (ActiveWorldIndex > 0);
            IncrementButton.interactable = (ActiveWorldIndex < DisplayChapters.Length - 1);
        }

        private void UpdateChapterButtons()
        {
            for (int n = 0; n < ChapterButtons.Length; n++)
            {
                ChapterButtons[n].WorldChapter = ActiveWorld.Chapters[n];
            }
        }
    }
}
