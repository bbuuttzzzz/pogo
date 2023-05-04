using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    [System.Serializable]
    public struct WorldChapter
    {
        [System.Serializable]
        public enum Types
        {
            Level,
            ComingSoon,
            SteamOnly
        }
        public Types Type;
        public ChapterDescriptor Chapter;
    }
}
