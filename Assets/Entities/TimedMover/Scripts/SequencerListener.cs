using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class SequencerListener : MonoBehaviour
{
    public Sequencer Target;
    public int TargetState;

    public UnityEvent OnSnap;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void Awake()
    {
        Target.OnSnapToState.AddListener(Target_OnSnapToState);
        Target.OnEnterState.AddListener(Target_OnEnterAnyState);
        Target.OnExitState.AddListener(Target_OnExitAnyState);
    }

    private void Target_OnSnapToState(int arg0)
    {
        throw new NotImplementedException();
    }

    private void Target_OnEnterAnyState(int arg0)
    {
        if (arg0 == TargetState) OnEnter.Invoke();
    }

    private void Target_OnExitAnyState(int arg0)
    {
        if (arg0 == TargetState) OnExit.Invoke();
    }

}
