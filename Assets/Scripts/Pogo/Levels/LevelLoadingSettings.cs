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
        /// the Scene to load
        /// </summary>
        public LevelDescriptor Level;
        /// <summary>
        /// allow reloading your current Scene. usually this warns and fails
        /// </summary>
        public bool ForceReload;
        /// <summary>
        /// Unload control scene after loading in, and reset stats
        /// </summary>
        public bool LoadingFromMenu;
        /// <summary>
        /// Instantly adjust the atmosphere on Scene load finish instead of slowly transitioning
        /// </summary>
        public bool Instantly;
        public QuickSaveData? QuickSaveData;
        /// <summary>
        /// Also set this MainLevelState after load
        /// </summary>
        public LevelState? MainLevelState;
        public LevelState[] AdditionalDefaultLevelStates;

        public static LevelLoadingSettings Default => new LevelLoadingSettings() { ForceReload = false };
        public static LevelLoadingSettings DefaultWithLevel(LevelDescriptor level)
        {
            var settings = Default;
            settings.Level = level;
            return settings;
        }
    }
}
