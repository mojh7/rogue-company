using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationHandler : MonoBehaviour {

    Character character;
    Animator animator;

    public void Init(Character character, RuntimeAnimatorController animatorController)
    {
        this.character = character;
        this.animator = GetComponent<Animator>();
        this.animator.runtimeAnimatorController = animatorController;
        Play();
        Idle();
    }

    public void Idle()
    {
        ResetAllParameter();
        animator.SetTrigger("idle");
    }

    public void Attack()
    {
        ResetAllParameter();
        animator.SetTrigger("attack");
    }

    public void Attacked()
    {
        ResetAllParameter();
        animator.SetTrigger("attaked");
    }

    public void Walk()
    {
        ResetAllParameter();
        animator.SetTrigger("walk");
    }

    public void Run()
    {
        ResetAllParameter();
        animator.SetTrigger("run");
    }

    public void Skill(int i)
    {
        ResetAllParameter();
        animator.SetInteger("skill", i);
    }

    public void Play()
    {
        ResetAllParameter();
        animator.enabled = true;
    }

    public void Stop()
    {
        animator.enabled = false;
    }

    private void ResetAllParameter()
    {
        animator.ResetTrigger("idle");
        animator.ResetTrigger("attack");
        animator.ResetTrigger("attacked");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("run");
        animator.SetInteger("skill", -1);
    }
    
    private void EndAnimation()
    {
        character.isCasting = false;
    }
}
