using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MovingPlatformController : MonoBehaviour
{
    public float CycleOffset;
    private void Awake()
    {
        GetComponent<Animator>().SetFloat("CycleOffset", CycleOffset);
        SetStartingPosition();
    }

    [ContextMenu("Set Starting Position")]
    private void SetStartingPosition()
    {
        var animator = GetComponent<Animator>();
        animator.Play("Cycle", 0, CycleOffset);
        animator.Update(0);
    }
}
