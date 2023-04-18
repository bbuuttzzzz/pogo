using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedBinaryStateMachine : MonoBehaviour
{
    public bool CallOnAwake;
    public int State;

    public UnityEvent EnterStateZero;
    public UnityEvent EnterStateOne;

    public float StateZeroTime = 2;
    public float StateOneTime = 2;

    private float nextTime;
    private int nextState = 1;

    private void Awake()
    {
        if (CallOnAwake)
        {
            EnterStateZero?.Invoke();
        }

        nextTime = Time.time + StateZeroTime;
        nextState = 1;
    }

    private void Update()
    {
        if (nextTime < Time.time)
        {
            GoToNextState();
        }
    }

    private void GoToNextState()
    {
        if (nextState == 0)
        {
            EnterStateZero?.Invoke();
        }
        else
        {
            EnterStateOne?.Invoke();
        }
        nextTime = Time.time + (nextState == 0 ? StateZeroTime : StateOneTime);
        nextState = (nextState + 1) % 2;
    }
}
