using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class DifficultyListener : MonoBehaviour
    {
        public PogoGameManager.Difficulty TargetDifficulty;

        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        
        private void Start()
        {
            if (PogoGameManager.GameInstanceIsValid())
            {
                var difficulty = PogoGameManager.PogoInstance.CurrentDifficulty;
                if (difficulty == TargetDifficulty)
                {
                    OnEnter?.Invoke();
                }

                PogoGameManager.PogoInstance.OnDifficultyChanged.AddListener(onDifficultyChanged);
            }
            
        }

        private void onDifficultyChanged(DifficultyChangedEventArgs e)
        {
            if (e.InitialDifficulty == TargetDifficulty)
            {
                OnExit?.Invoke();
            }
            
            if(e.FinalDifficulty == TargetDifficulty)
            {
                OnEnter?.Invoke();
            }
        }
    }
}
