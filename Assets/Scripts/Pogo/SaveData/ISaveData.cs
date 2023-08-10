using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Saving
{
    public interface ISaveData
    {
        public ChapterProgressData[] ChapterProgress { get; set; }
    }
}