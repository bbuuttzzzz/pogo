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
        public ChapterDescriptor Chapter;
        public GameObject TitleCardPrefab;

        public void EnterPortal()
        {
            if (!Chapter.IsUnlocked)
            {
                UnlockChapter();
            }
            ShowTitle();
        }

        private void ShowTitle()
        {
            var titleInstance = UIManager.Instance.SpawnUIElement(TitleCardPrefab);
            titleInstance.GetComponent<TitleCardController>().DisplayTitle(Chapter.Title);
        }

        private void UnlockChapter()
        {
            if (Chapter.UnlockedSaveValue == null) throw new MissingFieldException();
            Chapter.UnlockedSaveValue.CurrentValue = "1";
        }
    }
}
