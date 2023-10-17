using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Levels
{
    public class LevelStateChanger : MonoBehaviour
    {
        public LevelDescriptor Level;

        public void GoToState(int state)
        {
            PogoGameManager.PogoInstance.SetLevelState(new LevelState()
            {
                Level = Level,
                StateId = state
            });
        }
    }
}
