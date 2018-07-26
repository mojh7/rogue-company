using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationHandler : MonoBehaviour {

    Animator animator;

    public void Init(RuntimeAnimatorController  animatorController)
    {
        this.animator = GetComponent<Animator>();
        this.animator.runtimeAnimatorController = animatorController;
        animator.SetTrigger("idle");
    }
    
    public void Idle()
    {
        animator.SetTrigger("idle");
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void Attacked()
    {
        animator.SetTrigger("attaked");
    }

    public void Walk()
    {
        animator.SetTrigger("walk");
    }

    public void Run()
    {
        animator.SetTrigger("run");
    }

}
