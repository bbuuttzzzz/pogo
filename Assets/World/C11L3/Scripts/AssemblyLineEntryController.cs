using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.AssemblyLines
{
    [RequireComponent(typeof(Animator))]
    public class AssemblyLineEntryController : MonoBehaviour
    {
        Animator animator;
        public float CycleOffset;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            animator.SetFloat("CycleOffset", CycleOffset);
        }

        [ContextMenu("Set Starting Position")]
        public void SetStartingPosition()
        {
            animator = GetComponent<Animator>();
            animator.Play("Cycle", 0, CycleOffset);
            animator.Update(0);
        }
    }
}