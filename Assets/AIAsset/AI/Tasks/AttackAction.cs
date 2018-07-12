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
    public override bool Run()
    {
        return false;
    }
    public override Task Clone()
    {
        AttackAction parent = new AttackAction();

        return parent;
    }
}
