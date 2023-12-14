using Pogo.Collectibles;
using Pogo.Difficulties;
using System;
using WizardUtils;

namespace Pogo.Saving
{
    public abstract class SaveSlotDataTracker
    {
        public SaveSlotData SlotData;
        public enum DataStates
        {
            Unloaded,
            Loaded,
            Empty,
            Corrupt
        }
        public DataStates DataState;
        public abstract void Save();
        public abstract void Load(bool createIfEmpty = false);
        public abstract void Delete();
        public abstract void InitializeNew(string name, Difficulty difficulty);

        public SaveSlotPreviewData PreviewData
        {
            get
            {
                if (DataState != DataStates.Loaded)
                {
                    throw new InvalidOperationException();
                }

                return SlotData.previewData;
            }
            set
            {
                SlotData.previewData = value;
            }
        }

        const int TotalCompletionFromChapters = 900;
        const int TotalCompletionFromCollectibles = 100;

        public void UpdatePreviewData(CollectibleManifest collectibleManifest, WorldDescriptor world)
        {
            UpdatePreviewDataCollectibleCounts(collectibleManifest);

            SaveSlotPreviewData updatedPreviewData = PreviewData;
            updatedPreviewData.TotalDeaths = 0;
            updatedPreviewData.TotalMilliseconds = 0;
            updatedPreviewData.ChapterProgresses = new float[12];

            for (int n = 0; n < 12; n++)
            {
                if (SlotData.chapterProgressDatas[0,n].complete)
                {
                    updatedPreviewData.LastFinishedChapter = n+1;
                }
                if (!SlotData.chapterProgressDatas[0,n].unlocked) break;
                updatedPreviewData.TotalDeaths += SlotData.chapterProgressDatas[0, n].deathsTracked;
                updatedPreviewData.TotalMilliseconds += SlotData.chapterProgressDatas[0, n].millisecondsElapsed;
                updatedPreviewData.ChapterProgresses[n] = CalculateCollectibleProgress(world.FindChapter(n).Chapter);
            }


            updatedPreviewData.CompletionPerMille = (int)
                (
                    (updatedPreviewData.LastFinishedChapter / 12f) * TotalCompletionFromChapters
                    + (updatedPreviewData.TotalCollectibles / (float)collectibleManifest.Collectibles.Length) * TotalCompletionFromCollectibles
                );


            PreviewData = updatedPreviewData;
        }

        private float CalculateCollectibleProgress(ChapterDescriptor chapter)
        {
            int foundCollectibles = 0;
            foreach (var collectible in chapter.AssociatedCollectibles)
            {
                CollectibleUnlockData unlockData = GetCollectible(collectible.Key);
                if (unlockData.isUnlocked)
                {
                    foundCollectibles++;
                }
            }

            return (float) foundCollectibles / chapter.AssociatedCollectibles.Length;
        }

        private void UpdatePreviewDataCollectibleCounts(CollectibleManifest collectibleManifest)
        {
            SaveSlotPreviewData data = PreviewData;

            data.CollectedCoins = 0;
            data.TotalCollectibles = 0;

            foreach(var collectible in collectibleManifest.Collectibles)
            {
                CollectibleUnlockData unlockData = GetCollectible(collectible.Key);
                if (unlockData.isUnlocked)
                {
                    data.TotalCollectibles++;
                    if (collectible.CollectibleType == CollectibleDescriptor.CollectibleTypes.Coin)
                    {
                        data.CollectedCoins++;
                    }
                }
            }

            PreviewData = data;
            
        }

        public ChapterSaveData GetChapterProgressData(ChapterId id)
        {
            return SlotData.chapterProgressDatas[id.WorldNumber, id.ChapterNumber];
        }

        public void SetChapterProgressData(ChapterId id, ChapterSaveData data)
        {
            SlotData.chapterProgressDatas[id.WorldNumber, id.ChapterNumber] = data;
        }
        
        public void RollbackQuicksaveProgress()
        {
            var newPreviewData = SlotData.previewData;
            var newChapterData = GetChapterProgressData(SlotData.quickSaveData.ChapterId);

            newPreviewData.TotalMilliseconds -= SlotData.quickSaveData.SessionProgressTimeMilliseconds;
            newPreviewData.TotalDeaths -= SlotData.quickSaveData.SessionProgressDeaths;

            newChapterData.millisecondsElapsed -= SlotData.quickSaveData.ChapterProgressTimeMilliseconds;
            newChapterData.deathsTracked -= SlotData.quickSaveData.ChapterProgressDeaths;

            SlotData.previewData = newPreviewData;
            SetChapterProgressData(SlotData.quickSaveData.ChapterId, newChapterData);

            SlotData.quickSaveData = new QuickSaveData()
            {
                CurrentState = QuickSaveData.States.NoData
            };
        }

        public int IndexOfCollectible(string collectibleId)
        {
            for (int n = 0; n < SlotData.collectibleUnlockDatas.Length; n++)
            {
                if (SlotData.collectibleUnlockDatas[n].key == collectibleId)
                {
                    return n;
                }
            }

            return -1;
        }

        public CollectibleUnlockData GetCollectible(string collectibleId)
        {
            int id = IndexOfCollectible(collectibleId);
            if (id == -1)
            {
                CollectibleUnlockData newCollectible = new CollectibleUnlockData()
                {
                    key = collectibleId,
                    isUnlocked = false
                };
                return newCollectible;
            }
            else
            {
                return SlotData.collectibleUnlockDatas[id];
            }
        }

        public void SetCollectible(CollectibleUnlockData data)
        {
            int id = IndexOfCollectible(data.key);
            if (id == -1)
            {
                ArrayHelper.InsertAndResize(ref SlotData.collectibleUnlockDatas, data);
            }
            else
            {
                SlotData.collectibleUnlockDatas[id] = data;
            }
        }
    }
}