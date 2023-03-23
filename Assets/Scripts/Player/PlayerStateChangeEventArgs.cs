using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player
{
    public struct PlayerStateChangeEventArgs
    {
        public PlayerStates OldState;
        public PlayerStates NewState;

        public PlayerStateChangeEventArgs(PlayerStates oldState, PlayerStates newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
