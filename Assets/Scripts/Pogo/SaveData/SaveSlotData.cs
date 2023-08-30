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
        public ChapterProgressData[,] chapterProgressDatas;

        public static SaveSlotData NewGameData(string name, Difficulties difficulty)
        {
            var data = new SaveSlotData()
            {
                previewData = new SaveSlotPreviewData()
                {
                    difficulty = difficulty,
                    name = name
                },
                quickSaveData = new QuickSaveData() { },
                chapterProgressDatas = new ChapterProgressData[1,12]
            };

            for (int n = 0; n < 12; n++)
            {
                data.chapterProgressDatas[0,n] = new ChapterProgressData()
                {
                    unlocked = n == 0
                };
            }

            return data;
        }
    }
}
