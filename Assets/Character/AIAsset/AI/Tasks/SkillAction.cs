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
    Character other;
    AttackPattern attackPattern;
    public float Value
    {
        get
        {
            return Idx;
        }
    }

    public Task SetValue(int Idx)
    {
        this.Idx = Idx;
        return this;
    }

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.other = RootTask.BlackBoard["Target"] as Character;
        this.isRun = false;
        attackPattern = character.GetCharacterComponents().AIController.AttackPattern;
    }
    public override State Run()
    {
        if (!isRun)
        {
            isRun = true;
            return attackPattern.CastingSKill(character, other, Idx);
        }
        else
        {
            if(this.character.isCasting)
            {
                return State.CONTINUE;
            }
            else
            {
                isRun = false;
                return State.FAILURE;
            }
        }
    }
    public override bool SubRun()
    {
        return true;
    }
    public override Task Clone()
    {
        SkillAction parent = ScriptableObject.CreateInstance<SkillAction>();
        parent.Set(Probability);
        parent.SetValue(Idx);
        return parent;
    }
}
