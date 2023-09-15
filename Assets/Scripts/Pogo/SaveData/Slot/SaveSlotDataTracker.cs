using Assets.Scripts.Pogo.Difficulty;
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
        public abstract void InitializeNew(string name, DifficultyId difficulty);

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

        const float TotalCompletionFromChapters = 1f;

        public void UpdatePreviewData()
        {
            SaveSlotPreviewData updatedPreviewData = PreviewData;
            updatedPreviewData.TotalDeaths = 0;
            updatedPreviewData.TotalMilliseconds = 0;

            for (int n = 0; n < 12; n++)
            {
                if (SlotData.chapterProgressDatas[0,n].complete)
                {
                    updatedPreviewData.LastFinishedChapter = n;
                }
                if (!SlotData.chapterProgressDatas[0,n].unlocked) break;
                updatedPreviewData.TotalDeaths += SlotData.chapterProgressDatas[0, n].deathsTracked;
                updatedPreviewData.TotalMilliseconds += SlotData.chapterProgressDatas[0, n].millisecondsElapsed;
            }

            updatedPreviewData.CompletionPerMille = (int)
                (
                    (updatedPreviewData.LastFinishedChapter / 12f) * 1000 * TotalCompletionFromChapters
                );

            PreviewData = updatedPreviewData;
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