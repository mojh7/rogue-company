using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 A* 추적 행동을 담은 노드입니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/PositionTrackAction")]
public class PositionTrackAction : ActionTask
{
    [SerializeField]
    float doublingValue;
    [SerializeField]
    float radius;

    MovingPattern movingPattern;
    Vector2 destPosition;
    bool isArrived;

    public Task Set(float doublingValue,float radius)
    {
        if (doublingValue <= 1)
            doublingValue = 1;
        this.doublingValue = doublingValue;
        this.radius = radius;
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
        if(isArrived)
        {
            destPosition = RoomManager.Instance.GetNearestAvailableArea(character.transform.position + UnityEngine.Random.onUnitSphere * radius);
        }

        bool success = movingPattern.PositionTracking(destPosition,ref isArrived);

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
        PositionTrackAction parent = ScriptableObject.CreateInstance<PositionTrackAction>();
        parent.Set(doublingValue, radius);
        return parent;
    }
}