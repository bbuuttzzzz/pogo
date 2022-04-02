using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public UnityEvent OnOpenGamemodeScreen;
        public UnityEvent OnCloseGamemodeScreen;

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

        public void OpenGamemodeScreen()
        {
            OnOpenGamemodeScreen?.Invoke();
        }

        public void CloseGamemodeScreen()
        {
            OnCloseGamemodeScreen?.Invoke();
        }

        public void StartGame()
        {
            ChapterSelector.LoadActiveChapter();
        }
    }
}
