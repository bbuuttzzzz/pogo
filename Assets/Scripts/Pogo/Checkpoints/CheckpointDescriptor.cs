using Pogo.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Checkpoints
{
    [CreateAssetMenu(fileName = "ck_", menuName = "Pogo/CheckpointDescriptor")]
    public class CheckpointDescriptor : ScriptableObject
    {
        public LevelState LevelState;
        public LevelDescriptor Level;
        public int Par;
        public string[] Hints;

        public bool CanSkip;
        public CheckpointDescriptor OverrideSkipToCheckpoint;

        #region Cached Data
        [Tooltip("Controlled by ChapterDescriptor!!! do not touch!!!")]
        public ChapterDescriptor Chapter;
        [Tooltip("Controlled by ChapterDescriptor!!! do not touch!!!")]
        public CheckpointId CheckpointId;
        #endregion

        public override string ToString()
        {
            return $"Checkpoint {Chapter.name} {CheckpointId}";
        }
    }
}
