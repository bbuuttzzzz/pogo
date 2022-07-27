using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    public struct LevelLoadingSettings
    {
        public bool ForceReload;
        public bool LoadingFromMenu;
        public bool InstantChangeAtmosphere;

        public static LevelLoadingSettings Default => new LevelLoadingSettings() { ForceReload = false };
    }
}
