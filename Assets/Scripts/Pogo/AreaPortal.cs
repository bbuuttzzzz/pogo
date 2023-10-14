using Pogo.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo
{
    public class AreaPortal : MonoBehaviour
    {
        public LevelState LevelState;

        public void EnterPortal()
        {
            PogoGameManager.PogoInstance.LoadLevel(LevelState);
        }
    }
}