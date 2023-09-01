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

        public void EnterPortal()
        {
            if (!PogoGameManager.PogoInstance.CanSwitchChapters
                || PogoGameManager.PogoInstance.CurrentChapter == NextChapter)
            {
                return;
            }

            PogoGameManager.PogoInstance.StartChapter(NextChapter);
        }
    }
}
