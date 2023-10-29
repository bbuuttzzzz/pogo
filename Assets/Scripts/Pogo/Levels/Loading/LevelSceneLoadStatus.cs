using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Levels.Loading
{
    public struct LevelSceneLoadStatus
    {
        public enum States
        {
            NotLoaded,
            Loaded,
            Loading,
            Unloading
        }

        public States State;
        public float Progress;
    }
}
