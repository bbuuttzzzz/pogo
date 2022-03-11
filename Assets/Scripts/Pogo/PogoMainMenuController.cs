using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class PogoMainMenuController : MonoBehaviour
    {
        public LevelDescriptor InitialLevel;
        public void LoadInitialLevel()
        {
            PogoGameManager.PogoInstance.LoadLevel(InitialLevel, true);
            PogoGameManager.TryRegisterRespawnPoint(PogoGameManager.PogoInstance.InitialRespawnPoint);
            PogoGameManager.KillPlayer();
        }
    }
}
