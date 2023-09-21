using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishlistButtonController : MonoBehaviour
{
    public Animator animator;
    private void Start()
    {
        animator.SetBool("Flash", true);
    }
}
