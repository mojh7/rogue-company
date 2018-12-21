using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 A* 추적 행동을 담은 노드입니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/AStarTrackAtion")]
public class AStarTrackAtion : ActionTask
{
    [SerializeField]
    float doublingValue;

    MovingPattern movingPattern;
    public Task SetValue(float doublingValue)
    {
        if (doublingValue <= 1)
            doublingValue = 1;
        this.doublingValue = doublingValue;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.target = RootTask.BlackBoard["Target"] as Character;
        movingPattern = character.GetCharacterComponents().AIController.MovingPattern;
        movingPattern.AStarTracker(target.transform, doublingValue);
    }
    public override State Run()
    {
        if (character.isCasting)
            return State.FAILURE;
        character.SetAutoAim();
        bool success = movingPattern.AStarTracking();

        if (success)
        {
            animationHandler.Walk();
            return State.SUCCESS;
        }
        else
        {
            return State.FAILURE;
        }
    }
    public override Task Clone()
    {
        AStarTrackAtion parent = ScriptableObject.CreateInstance<AStarTrackAtion>();
        parent.Set(Probability);
        parent.SetValue(doublingValue);
        return parent;
    }
}