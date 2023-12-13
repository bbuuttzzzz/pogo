using Pogo.Checkpoints;
using Pogo.Collectibles;
using Pogo.Levels;
using System;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu(fileName = "ChapterDescriptor", menuName = "ScriptableObjects/ChapterDescriptor", order = 1)]
    public class ChapterDescriptor : ScriptableObject
    {
        public LevelDescriptor Legacy_Level;

        public int Number;
        public string Title;
        public Sprite Icon;
        public string LongTitle => $"Part {Number} - {Title}";

        public CollectibleDescriptor[] AssociatedCollectibles;

        public CheckpointDescriptor GetCheckpointDescriptor(CheckpointId checkpointId)
        {
            if (checkpointId.CheckpointType == CheckpointTypes.MainPath)
            {
                if (checkpointId.CheckpointNumber - 1 > MainPathCheckpoints.Length - 1)
                {
                    throw new IndexOutOfRangeException($"CheckpointId {checkpointId} out of range for Chapter {name}");
                }
                return MainPathCheckpoints[checkpointId.CheckpointNumber - 1];
            }
            else
            {
                if (checkpointId.CheckpointNumber - 1 > SidePathCheckpoints.Length - 1)
                {
                    throw new IndexOutOfRangeException($"CheckpointId {checkpointId} out of range for Chapter {name}");
                }
                return SidePathCheckpoints[checkpointId.CheckpointNumber - 1];
            }
        }

        public CheckpointDescriptor[] MainPathCheckpoints;
        public CheckpointDescriptor[] SidePathCheckpoints;

        private void OnValidate()
        {
            if (MainPathCheckpoints == null) MainPathCheckpoints = new CheckpointDescriptor[0];
            if (SidePathCheckpoints == null) SidePathCheckpoints = new CheckpointDescriptor[0];

            for (int n = 0; n < MainPathCheckpoints.Length; n++)
            {
                UpdateMainCheckpoint(n);
            }

            for (int n = 0; (n < SidePathCheckpoints.Length); n++)
            {
                UpdateSideCheckpoint(n);
            }
        }

        public void UpdateSideCheckpoint(int n)
        {
            if (SidePathCheckpoints[n] != null)
            {
                SidePathCheckpoints[n].Chapter = this;
                SidePathCheckpoints[n].CheckpointId = new CheckpointId(CheckpointTypes.SidePath, n + 1);
            }
        }

        public void UpdateMainCheckpoint(int n)
        {
            if (MainPathCheckpoints[n] != null)
            {
                MainPathCheckpoints[n].Chapter = this;
                MainPathCheckpoints[n].CheckpointId = new CheckpointId(CheckpointTypes.MainPath, n + 1);
            }
        }

        #region Unlocking
        public bool AlwaysUnlocked;
        public bool IsUnlocked
        {
            get
            {
                return AlwaysUnlocked
                    || PogoGameManager.PogoInstance.GetChapterSaveData(this).unlocked;
            }
            set
            {
                PogoGameManager.PogoInstance.UnlockChapter(this);
            }
        }

        #endregion
    }
}