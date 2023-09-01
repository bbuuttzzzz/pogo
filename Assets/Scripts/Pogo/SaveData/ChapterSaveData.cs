using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct ChapterSaveData
    {
        public bool unlocked;
        public bool complete;
        public long millisecondsElapsed;
        public int deathsTracked;
        public int millisecondsBestTime;
        public int bestDeaths;
    }
}
