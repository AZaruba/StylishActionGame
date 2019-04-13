using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {

    [SerializeField] private Animator animator;

    public void StartWalkAnimation()
    {
        animator.SetTrigger("StartWalk");
    }

    public void StartIdleAnimation()
    {
        animator.SetTrigger("StartIdle");
    }
}
