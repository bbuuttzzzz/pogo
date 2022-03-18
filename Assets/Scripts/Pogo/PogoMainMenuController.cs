using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public ChapterDescriptor InitialChapter;
        public void LoadInitialChapter()
        {
            PogoGameManager.PogoInstance.LoadChapter(InitialChapter);
        }
    }
}
