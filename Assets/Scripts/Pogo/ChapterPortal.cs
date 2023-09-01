using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUI;

namespace Pogo
{
    public class ChapterPortal : MonoBehaviour
    {
        public ChapterDescriptor PreviousChapter;
        public ChapterDescriptor NextChapter;
        public GameObject TitleCardPrefab;

        public void EnterPortal()
        {
            if (!PogoGameManager.PogoInstance.CanSwitchChapters
                || PogoGameManager.PogoInstance.CurrentChapter == NextChapter)
            {
                return;
            }

            if (PreviousChapter != null)
            {
                FinishPreviousChapter();
            }

            if (!NextChapter.IsUnlocked)
            {
                UnlockChapter();
            }
            ShowTitle();
        }

        private void FinishPreviousChapter()
        {
            if (PreviousChapter == null) return;
            PogoGameManager.PogoInstance.FinishChapter(PreviousChapter);
        }

        private void ShowTitle()
        {
            var titleInstance = UIManager.Instance.SpawnUIElement(TitleCardPrefab);
            titleInstance.GetComponent<TitleCardController>().DisplayTitle(NextChapter.Title);
        }

        private void UnlockChapter()
        {
            NextChapter.IsUnlocked = true;
        }
    }
}
