using Pogo.Levels;
using Pogo.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    public struct LevelLoadingSettings
    {
        /// <summary>
        /// allow reloading your current level. usually this warns and fails
        /// </summary>
        public bool ForceReload;
        /// <summary>
        /// Unload control scene after loading in, and reset stats
        /// </summary>
        public bool LoadingFromMenu;
        /// <summary>
        /// Instantly adjust the atmosphere on level load finish instead of slowly transitioning
        /// </summary>
        public bool Instantly;
        public QuickSaveData? QuickSaveData;
        public LevelState LevelState;

        public static LevelLoadingSettings Default => new LevelLoadingSettings() { ForceReload = false };
        public static LevelLoadingSettings DefaultWithState(LevelState levelState)
        {
            var settings = Default;
            settings.LevelState = levelState;
            return settings;
        }
    }
}
