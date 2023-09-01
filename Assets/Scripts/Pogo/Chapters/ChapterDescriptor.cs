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
        public LevelDescriptor Level;

        public int Number;
        public string Title;
        public Sprite Icon;
        public string LongTitle => $"Part {Number} - {Title}";

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