using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventReplicator : MonoBehaviour
{
    public UnityEvent OnCalled;

    public void Call() => OnCalled?.Invoke();

    public void DelayedCall(float delaySeconds)
    {
        StartCoroutine(CallDelayed(delaySeconds));
    }

    public void DelayedCallRealtime(float delaySeconds)
    {
        StartCoroutine(CallDelayed(delaySeconds, true));
    }

    private IEnumerator CallDelayed(float delaySeconds, bool realtime = false)
    {
        if (realtime)
        {
            yield return new WaitForSecondsRealtime(delaySeconds);
        }
        else
        {
            yield return new WaitForSeconds(delaySeconds);
        }

        Call();
    }
}
