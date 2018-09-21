using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationHandler : MonoBehaviour {

    [SerializeField]
    Animator headAnim;
    Character character;
    Animator animator;
    System.Action lapsedAction, endAction;

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
        if(headAnim)
            headAnim.SetTrigger("idle");
    }

    public void Attack()
    {
        ResetAllParameter();
        animator.SetTrigger("attack");
        if (headAnim)
            headAnim.SetTrigger("attack");

    }

    public void Attacked()
    {
        ResetAllParameter();
        animator.SetTrigger("attaked");
        if (headAnim)
            headAnim.SetTrigger("attaked");
    }

    public void Walk()
    {
        ResetAllParameter();
        animator.SetTrigger("walk");
        if (headAnim)
            headAnim.SetTrigger("walk");
    }

    public void Run()
    {
        ResetAllParameter();
        animator.SetTrigger("run");
        if (headAnim)
            headAnim.SetTrigger("run");
    }

    public void Skill(int i)
    {
        ResetAllParameter();
        animator.SetInteger("skill", i);
        if (headAnim)
            headAnim.SetInteger("skill", i);
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

    public void SetLapsedAction(System.Action action)
    {
        this.lapsedAction = action;
    }

    public void SetEndAction(System.Action action)
    {
        this.endAction = action;
    }

    private void ResetAllParameter()
    {
        animator.ResetTrigger("idle");
        animator.ResetTrigger("attack");
        animator.ResetTrigger("attacked");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("run");
        animator.SetInteger("skill", -1);

        if (headAnim)
        {
            headAnim.ResetTrigger("idle");
            headAnim.ResetTrigger("attack");
            headAnim.ResetTrigger("attacked");
            headAnim.ResetTrigger("walk");
            headAnim.ResetTrigger("run");
            headAnim.SetInteger("skill", -1);
        }

    }

    private void LapseAnimation()
    {
        if (lapsedAction != null)
            lapsedAction.Invoke();
    }

    private void EndAnimation()
    {
        if(endAction != null)
            endAction.Invoke();
        character.isCasting = false;
    }
}
