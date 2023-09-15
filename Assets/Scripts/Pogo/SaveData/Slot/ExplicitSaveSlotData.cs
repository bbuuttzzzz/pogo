using Pogo.Collectibles;
using UnityEngine;

namespace Pogo.Saving
{
    [CreateAssetMenu(fileName = "saveSlotData_", menuName = "Pogo/Saving/SaveSlotDataExplicit", order = 1)]
    public class ExplicitSaveSlotData : ScriptableObject
    {
        public SaveSlotPreviewData previewData;
        public QuickSaveData quickSaveData;
        public CollectibleDescriptor[] unlockedCollectibles;

        public SaveSlotData Data
        {
            get
            {
                SaveSlotData data = new SaveSlotData()
                {
                    quickSaveData = this.quickSaveData,
                    previewData = this.previewData,
                    chapterProgressDatas = new ChapterSaveData[1, 12],
                    collectibleUnlockDatas = new CollectibleUnlockData[unlockedCollectibles.Length],
                };
                for (int n = 0; n < 12; n++)
                {
                    data.chapterProgressDatas[0, n] = new ChapterSaveData()
                    {
                        unlocked = n <= previewData.LastFinishedChapter + 1,
                        complete = n <= previewData.LastFinishedChapter,
                        bestDeaths = Random.Range(0, 100),
                        millisecondsBestTime = Random.Range(1000, 600000),
                        deathsTracked = Random.Range(0, 1000),
                        millisecondsElapsed = Random.Range(1000, 6000000)
                    };
                }
                for (int n = 0; n < unlockedCollectibles.Length; n++)
                {
                    data.collectibleUnlockDatas[n] = new CollectibleUnlockData()
                    {
                        key = unlockedCollectibles[n].Key,
                        isUnlocked = true
                    };
                }

                return data;
            }
        }
    }
}
