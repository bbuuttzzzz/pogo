using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

public class Sequencer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float CycleOffset;

    private int currentState;
    private int CurrentState
    {
        get => currentState;
        set
        {
            if (currentState >= 0)
            {
                OnExitState?.Invoke(currentState);
            }
            currentState = value;
            if (value >= 0)
            {
                OnEnterState?.Invoke(value);
            }
        }
    }

    public State[] States;

    private float TotalDurationSeconds
    {
        get
        {
            float sum = 0;
            foreach(var state in States)
            {
                sum += state.Duration;
            }
            return sum;
        }
    }

    public UnityEvent<int> OnSnapToState;
    public UnityEvent<int> OnEnterState;
    public UnityEvent<int> OnExitState;

    private float nextTime;
    private int nextState;

    private void Awake()
    {
        int initialState = 0;
        float offset = TotalDurationSeconds * CycleOffset;
        while (States[initialState].Duration < offset)
        {
            offset -= States[initialState].Duration;
            initialState = (CurrentState + 1) % States.Length;
        }

        BeginState(offset);
    }

    private void BeginState(float timeOffset = 0)
    {
        nextTime = Time.time + States[CurrentState].Duration - timeOffset;
        nextState = (CurrentState + 1) % States.Length;
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
        CurrentState = nextState;
        BeginState();
    }

    [System.Serializable]
    public struct State
    {
        public float Duration;
    }
}
