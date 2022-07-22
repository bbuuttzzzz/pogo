using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Logic
{
    public class StatsHUDController : MonoBehaviour
    {
        public int QuickDisplayInterval = 10;

        Animator animator;
        private void Start()
        {
            animator = GetComponent<Animator>();
            PogoGameManager.PogoInstance.OnPlayerDeath.AddListener(onDeath);
            PogoGameManager.PogoInstance.OnPauseStateChanged += onPauseStateChanged;
        }

        private void onPauseStateChanged(object sender, bool e)
        {
            animator.SetBool("DisplayDeaths", e);
        }

        private void onDeath()
        {
            DeathCount = PogoGameManager.PogoInstance.NumberOfDeaths;
            OnDeathCountChanged?.Invoke(DeathCount);
            if (DeathCount % QuickDisplayInterval == 0)
            {
                OnDeathCountChangedLargeInterval?.Invoke();
            }
        }

        public int DeathCount;
        public UnityEvent<int> OnDeathCountChanged;
        public UnityEvent OnDeathCountChangedLargeInterval;
    }
}
