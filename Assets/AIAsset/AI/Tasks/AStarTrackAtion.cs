using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 A* 추적 행동을 담은 노드입니다.
/// </summary>
public class AStarTrackAtion : ActionTask
{
    MovingPattern movingPattern;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.target = RootTask.BlackBoard["Target"] as Character;
        movingPattern = character.GetCharacterComponents().AIController.MovingPattern;
        movingPattern.AStarTracker(target.transform);
    }
    public override bool Run()
    {
        success = movingPattern.AStarTracking();

        if (success)
        {
            animationHandler.Walk();
            return true;
        }
        else
        {
            return false;
        }
    }
    public override Task Clone()
    {
        AStarTrackAtion parent = new AStarTrackAtion();

        return parent;
    }
}