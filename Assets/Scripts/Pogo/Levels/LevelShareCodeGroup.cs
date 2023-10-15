using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Levels
{
    public class LevelShareCodeGroup
    {
        public LevelDescriptor Level;
        public List<ShareCode> Codes;

        public bool TryGetCode(LevelState state, out ShareCode code)
        {
            foreach(var _code in Codes)
            {
                if (_code.LevelState == state)
                {
                    code = _code;
                    return true;
                }
            }

            code = default;
            return false;
        }
    }
}
