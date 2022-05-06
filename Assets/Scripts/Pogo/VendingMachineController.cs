using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils.Saving;

namespace Pogo
{
    public class VendingMachineController : MonoBehaviour
    {
        public UnlockCounter QuarterCounter;

        public UnityEvent OnSpawnChips;
        public UnityEvent OnShowHint;

        bool chipsSpawned;
        float lastCheck;
        float checkInterval = 5;

        private void Awake()
        {
            lastCheck = Time.time - checkInterval;
        }

        public void Check()
        {
            if (chipsSpawned || lastCheck + checkInterval > Time.time) return;
            lastCheck = Time.time;

            int count = QuarterCounter.Check();
            int maxCount = QuarterCounter.Saves.Length;

            if (count >= maxCount)
            {
                chipsSpawned = true;
                OnSpawnChips?.Invoke();
            }
            else
            {
                OnShowHint?.Invoke();
            }
        }

    }
}
