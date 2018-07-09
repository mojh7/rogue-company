using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationHandler : MonoBehaviour {

    Animator animator;
    CharacterInfo.charType charType;
    private void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    public void Init(CharacterInfo.charType charType)
    {
        this.charType = charType;
        animator.SetTrigger("idle");
    }

    public void Attack(uint i)
    {
        animator.SetTrigger("attack");
    }

    public void Attacked(uint i)
    {
        animator.SetTrigger("attaked");
    }

    public void Walk(uint i)
    {
        animator.SetTrigger("walk");
    }

    public void Run(uint i)
    {
        animator.SetTrigger("run");
    }

}
