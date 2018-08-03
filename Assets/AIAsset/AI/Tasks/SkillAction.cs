using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/SkillAction")]
public class SkillAction : ActionTask
{
    int Idx;
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
    public override bool Run()
    {
        success = attackPattern.CastingSKill(character, 1, 0);

        return true;
    }
    public override Task Clone()
    {
        SkillAction parent = new SkillAction();

        return parent;
    }
}
