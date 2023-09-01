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
        public abstract void InitializeNew(string name, Difficulties difficulty);

        public SaveSlotPreviewData GetPreviewData()
        {
            if (DataState != DataStates.Loaded)
            {
                throw new InvalidOperationException();
            }

            return SlotData.previewData;
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