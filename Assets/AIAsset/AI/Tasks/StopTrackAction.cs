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
    MovingPattern movingPattern;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        movingPattern = character.GetCharacterComponents().AIController.MovingPattern;
        movingPattern.StopTracker(character.transform);
    }

    public override bool Run()  
    {
        success = movingPattern.StopTracking();

        if (success)
        {
            animationHandler.Idle();
            return true;
        }
        else
        {
            return false;
        }
    }

    public override Task Clone()
    {
        StopTrackAction parent = new StopTrackAction();

        return parent;
    }
}
