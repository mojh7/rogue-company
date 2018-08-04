using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 캐릭터 사망시 행동을 담은 노드입니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/CharacterDeadAction")]
public class CharacterDeadAction : ActionTask
{
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
    }
    public override State Run()
    {
        if (character.IsDie())
        {
            return State.SUCCESS;
        }
        else
        {
            return State.FAILURE;
        }
    }
    public override Task Clone()
    {
        CharacterDeadAction parent = new CharacterDeadAction();

        return parent;
    }
}