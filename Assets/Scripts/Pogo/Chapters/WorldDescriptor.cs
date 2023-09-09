using Pogo.Saving;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.Saving;

namespace Pogo
{
    [CreateAssetMenu(fileName = "WorldDescriptor", menuName = "ScriptableObjects/WorldDescriptor", order = 1)]
    public class WorldDescriptor : ScriptableObject
    {
        private const int ChapterCount = 12;
        public WorldChapter[] Chapters = new WorldChapter[ChapterCount];

        public string DisplayName;

        public int IndexOf(ChapterDescriptor chapterDescriptor)
        {
            for (int n = 0; n < ChapterCount; n++)
            {
                if (Chapters[n].Type == WorldChapter.Types.Level
                    && Chapters[n].Chapter == chapterDescriptor)
                {
                    return n;
                }
            }

            return -1;
        }

        public WorldChapter FindChapter(int index)
        {
            return Chapters[index];
        }

        void OnValidate()
        {
            if (Chapters.Length != ChapterCount)
            {
                Debug.LogWarning("Don't change the 'Levels' field's array size!");
                Array.Resize(ref Chapters, ChapterCount);
            }
        }
    }
}
