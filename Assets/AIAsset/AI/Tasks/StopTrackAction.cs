using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

// 0720 : 기획자님 왈 : 정지 상태에서는 원거리 공격만 합니다.

/// <summary>
///  몬스터의 정지상태를 나타내는 노드.
/// </summary>
public class StopTrackAction : ActionTask
{
    Character target;
    AIController controller;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        controller = character.GetCharacterComponents().AIController;
    }

    public override bool Run()  
    { 
        // 애니메이션 핸들러에서 애니메이션 멈추겠다는거 추가하기
        animationHandler.Idle();
        controller.StopMove();
        return base.Run();
    }

    public override Task Clone()
    {
        StopTrackAction parent = new StopTrackAction();

        return parent;
    }
}
