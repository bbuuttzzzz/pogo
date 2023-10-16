using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Levels
{
    public class LevelStateListener : MonoBehaviour
    {
        public LevelDescriptor TargetLevel;
        public UnityEvent<LevelStateChangedArgs> OnLevelStateChanged;

        public void Start()
        {
            PogoGameManager.PogoInstance.OnLevelStateChanged.AddListener(GameManager_OnLevelStateChanged);
        }

        private void GameManager_OnLevelStateChanged(LevelStateChangedArgs arg0)
        {
            if (arg0.NewState.Level == TargetLevel)
            {
                OnLevelStateChanged.Invoke(arg0);
            }
        }

        private void OnDestroy()
        {
            PogoGameManager.PogoInstance.OnLevelStateChanged.RemoveListener(GameManager_OnLevelStateChanged);
        }
    }
}