using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct SaveSlotData
    {
        public SaveSlotPreviewData? previewData;
        public QuickSaveData quickSaveData;
        public ChapterProgressData[] chapterProgressData;

        public static SaveSlotData NewGameData()
        {
            return new SaveSlotData()
            {
                quickSaveData = new QuickSaveData() { },
                chapterProgressData = new ChapterProgressData[] { },
            };
        }
    }
}
