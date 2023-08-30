using Assets.Scripts.Pogo.Difficulty;
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
        public SaveSlotPreviewData previewData;
        public QuickSaveData quickSaveData;
        public ChapterProgressData[] chapterProgressData;

        public static SaveSlotData NewGameData(string name, Difficulties difficulty)
        {
            return new SaveSlotData()
            {
                previewData = new SaveSlotPreviewData()
                {
                    difficulty = difficulty,
                    name = name
                },
                quickSaveData = new QuickSaveData() { },
                chapterProgressData = new ChapterProgressData[] { },
            };
        }
    }
}
