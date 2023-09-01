using Pogo.Challenges;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo
{
    public class PogoChapterSelectorController : MonoBehaviour
    {
        [HideInInspector]
        public ChapterDescriptor[] DisplayChapters;
        private int ActiveWorldIndex = 0;
        private WorldDescriptor ActiveWorld => PogoGameManager.PogoInstance.World;
        public ChapterButtonController[] ChapterButtons;

        private void Start()
        {
            PogoGameManager.PogoInstance.OnSaveSlotChanged.AddListener(GM_OnSaveSlotChanged);
        }

        private void GM_OnSaveSlotChanged()
        {
            UpdateChapterButtons();
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
