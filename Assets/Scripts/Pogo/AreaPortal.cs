using Pogo.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo
{
    public class AreaPortal : MonoBehaviour
    {
        public LevelDescriptor Level;

        [HideInInspector]
        public bool ShouldSetLevelState;
        [HideInInspector]
        public LevelState LevelState;
        
        public void EnterPortal()
        {
            LevelLoadingSettings settings = LevelLoadingSettings.DefaultWithLevel(Level);
            if (ShouldSetLevelState)
            {
                settings.LevelState = LevelState;
            }
            PogoGameManager.PogoInstance.LoadLevel(settings);
        }
    }
}