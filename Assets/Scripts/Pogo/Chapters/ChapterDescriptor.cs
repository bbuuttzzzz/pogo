using Pogo.Checkpoints;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.Saving;

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

        public CheckpointDescriptor GetCheckpoint(CheckpointId checkpointId)
        {
            if (checkpointId.CheckpointType == CheckpointTypes.MainPath)
            {
                return MainPathCheckpoints[checkpointId.CheckpointNumber - 1];
            }
            else
            {
                return SidePathCheckpoints[checkpointId.CheckpointNumber - 1];
            }
        }

        public CheckpointDescriptor[] MainPathCheckpoints;
        public CheckpointDescriptor[] SidePathCheckpoints;

        public ChapterStartPoint FindStartPoint()
        {
            var startPoints = FindObjectsOfType(typeof(ChapterStartPoint)) as ChapterStartPoint[];
            foreach (var startPoint in startPoints)
            {
                if (startPoint.Chapter == this)
                {
                    return startPoint;
                }
            }

            throw new MissingReferenceException($"Could not find ChapterStartPoint for ChapterDescriptor {name}");
        }

        private void OnValidate()
        {
            if (MainPathCheckpoints == null) MainPathCheckpoints = new CheckpointDescriptor[0];
            if (SidePathCheckpoints == null) SidePathCheckpoints = new CheckpointDescriptor[0];

            for (int n = 0; n < MainPathCheckpoints.Length; n++)
            {
                if (MainPathCheckpoints[n] == null) continue;

                MainPathCheckpoints[n].CheckpointId = new CheckpointId(CheckpointTypes.MainPath, n + 1);
            }

            for (int n = 0; (n < SidePathCheckpoints.Length); n++)
            {
                if (SidePathCheckpoints[n] == null) continue;

                SidePathCheckpoints[n].CheckpointId = new CheckpointId(CheckpointTypes.SidePath, n + 1);
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