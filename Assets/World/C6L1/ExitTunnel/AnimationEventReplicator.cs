using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventReplicator : MonoBehaviour
{
    public UnityEvent OnCalled;

    public void Call() => OnCalled?.Invoke();
}
