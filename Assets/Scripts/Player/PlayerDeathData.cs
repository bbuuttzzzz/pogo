using Pogo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public struct PlayerDeathData
    {
        public IKillType KillType;
        public Vector3? Position;
        public Vector3? Normal;

        public PlayerDeathData(IKillType killType) : this()
        {
            KillType = killType;
        }

        public PlayerDeathData(IKillType killType, Vector3 position, Vector3 normal) : this(killType)
        {
            Position = position;
            Normal = normal;
        }
    }
}
