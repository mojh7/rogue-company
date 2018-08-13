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
    bool isRun;
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
        this.isRun = false;
        this.temporary = null;
        attackPattern = character.GetCharacterComponents().AIController.AttackPattern;
    }
    public override State Run()
    {
        if (!isRun)
        {
            isRun = true;
            return attackPattern.CastingSKill(character, Idx, temporary);
        }
        else
        {
            if(this.character.isCasting)
            {
                return State.CONTINUE;
            }
            else
            {
                return State.FAILURE;
            }
        }
    }
    public override bool SubRun()
    {
        isRun = false;
        return true;
    }
    public override Task Clone()
    {
        SkillAction parent = ScriptableObject.CreateInstance<SkillAction>();
        parent.Set(Idx);
        return parent;
    }
}
