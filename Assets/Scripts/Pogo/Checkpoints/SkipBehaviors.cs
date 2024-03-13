using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Checkpoints
{
    [Serializable]
    public enum SkipBehaviors
    {
        LevelChange,
        TeleportToTarget
    }
}
