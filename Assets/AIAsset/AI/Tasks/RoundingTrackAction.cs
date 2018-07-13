using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 회전 추적 행동을 담은 노드입니다. 
/// </summary>
public class RoundingTrackAction : ActionTask
{
    MovingPattern movingPattern;
    Character target;

    [SerializeField]
    float radius;

    public Task Set(float radius)
    {
        this.radius = radius;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.target = RootTask.BlackBoard["Target"] as Character;
        movingPattern = character.GetCharacterComponents().AIController.MovingPattern;
        movingPattern.RoundingTracker(target.transform, radius);
    }
    public override bool Run()
    {
        success = movingPattern.RoundingTracking();
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
        RoundingTrackAction parent = new RoundingTrackAction();
        parent.Set(radius);
        return parent;
    }
}