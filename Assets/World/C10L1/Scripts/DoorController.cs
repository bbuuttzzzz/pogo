using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class DoorController : MonoBehaviour
{
    public UnityEvent OnEnterPositiveStateInstantly;
    public UnityEvent OnEnterNegativeStateInstantly;

    public UnityEvent OnEnterPositiveState;
    public UnityEvent OnEnterNegativeState;

    [SerializeField]
    private bool leverActive;
    [SerializeField]
    private bool overrideActive;

    private bool DoorIsOpen;
    private bool DoorShouldOpen => leverActive || overrideActive;

    private void Awake()
    {
        DoorIsOpen = false;
        Recalculate(true);
    }

    public void SetLeverStateInstantly(bool newState) => SetLeverState(newState, true);
    public void SetLeverState(bool newState) => SetLeverState(newState, false);
    public void SetOverrideStateInstantly(bool newState) => SetOverrideState(newState, true);
    public void SetOverrideState(bool newState) => SetOverrideState(newState, false);

    private void SetLeverState(bool newState, bool instant)
    {
        if (leverActive == newState) return;

        leverActive = newState;
        Recalculate(instant);
    }

    private void SetOverrideState(bool newState, bool instant)
    {
        if (overrideActive == newState) return;

        overrideActive = newState;
        Recalculate(instant);
    }

    private void Recalculate(bool instant)
    {
        if (DoorShouldOpen == DoorIsOpen) return;

        DoorIsOpen = DoorShouldOpen;

        if (DoorIsOpen)
        {
            if (instant)
            {
                OnEnterPositiveStateInstantly?.Invoke();
            }
            else
            {
                OnEnterPositiveState?.Invoke();
            }
        }
        else
        {

            if (instant)
            {
                OnEnterNegativeStateInstantly?.Invoke();
            }
            else
            {
                OnEnterNegativeState?.Invoke();
            }
        }
    }
}
