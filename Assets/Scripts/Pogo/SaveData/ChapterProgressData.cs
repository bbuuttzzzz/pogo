using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct ChapterProgressData
    {
        public bool complete;
        public int millisecondsElapsed;
        public int deathsTracked;
        public int millisecondsBestTime;
        public int bestDeaths;
    }
}
