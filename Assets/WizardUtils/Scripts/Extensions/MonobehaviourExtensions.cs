using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Extensions
{
    public static class MonobehaviourExtensions
    {
        public static Coroutine StartDelayCoroutineUnscaled(this MonoBehaviour self, float delaySeconds, Action callback)
        {
            return self.StartCoroutine(WaitForSecondsUnscaled(delaySeconds, callback));
        }

        private static IEnumerator WaitForSecondsUnscaled(float seconds, Action callback)
        {
            yield return new WaitForSecondsRealtime(seconds);

            callback();
        }
    }
}
