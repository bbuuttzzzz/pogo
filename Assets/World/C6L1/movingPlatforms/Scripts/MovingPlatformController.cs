using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MovingPlatformController : MonoBehaviour
{
    Animator animator;
    public float CycleOffset;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("CycleOffset", CycleOffset);
    }

    [ContextMenu("Set Starting Position")]
    private void SetStartingPosition()
    {
        animator = GetComponent<Animator>();
        animator.Play("Cycle", 0, CycleOffset);
        animator.Update(0);
    }
}
