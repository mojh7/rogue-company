using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 A* 추적 행동을 담은 노드입니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/MidTrackAction")]
public class MidTrackAction : ActionTask
{
    [SerializeField]
    float doublingValue;

    MovingPattern movingPattern;
    Vector2 destPosition;
    bool isArrived;

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
        movingPattern.PositionTracker(target.transform, doublingValue);
        isArrived = true;
    }
    public override State Run()
    {
        if (character.isCasting)
            return State.FAILURE;
        character.SetManualAim();
        if (isArrived)
        {
            destPosition = RoomManager.Instance.MidPosition;
        }

        bool success = movingPattern.PositionTracking(destPosition, ref isArrived);

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
        MidTrackAction parent = ScriptableObject.CreateInstance<MidTrackAction>();
        parent.Set(Probability);
        parent.SetValue(doublingValue);
        return parent;
    }
}
