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
        public ChapterSaveData[,] chapterProgressDatas;
        public CollectibleUnlockData[] collectibleUnlockDatas;

        public static SaveSlotData NewGameData(string name, DifficultyId difficulty)
        {
            var data = new SaveSlotData()
            {
                previewData = new SaveSlotPreviewData()
                {
                    difficulty = difficulty,
                    name = name
                },
                quickSaveData = new QuickSaveData() { },
                chapterProgressDatas = new ChapterSaveData[1, 12],
                collectibleUnlockDatas = new CollectibleUnlockData[0]
            };

            for (int n = 0; n < 12; n++)
            {
                data.chapterProgressDatas[0,n] = new ChapterSaveData()
                {
                    unlocked = n == 0
                };
            }

            return data;
        }
    }
}
