using Assets.Scripts.Pogo.Difficulty;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        public abstract void Load();
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

    }
}