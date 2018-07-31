using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 공격 행동을 담은 노드입니다.
/// </summary>
public class AttackAction : ActionTask
{
    AttackPattern attackPattern;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        attackPattern = character.GetCharacterComponents().AIController.AttackPattern;
    }
    public override bool Run()
    {
        attackPattern.Temp(character, 1, 1, 3, 10);
        return true;
    }
    public override Task Clone()
    {
        AttackAction parent = new AttackAction();

        return parent;
    }
}
