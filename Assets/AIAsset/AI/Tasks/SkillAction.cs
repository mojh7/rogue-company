using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/SkillAction")]
public class SkillAction : ActionTask
{
    [SerializeField]
    int Idx;
    object temporary;
    AttackPattern attackPattern;

    public Task Set(int Idx)
    {
        this.Idx = Idx;
        return this;
    }

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        attackPattern = character.GetCharacterComponents().AIController.AttackPattern;
    }
    public override State Run()
    {
        bool success = attackPattern.CastingSKill(character, Idx, temporary);

        return State.SUCCESS;
    }
    public override Task Clone()
    {
        SkillAction parent = new SkillAction();

        return parent;
    }
}
