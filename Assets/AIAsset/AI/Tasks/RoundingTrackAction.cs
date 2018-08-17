using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 회전 추적 행동을 담은 노드입니다. 
/// </summary>
[CreateAssetMenu(menuName = "Task/RoundingTrackAction")]
public class RoundingTrackAction : ActionTask
{
    [SerializeField]
    float doublingValue;
    MovingPattern movingPattern;

    [SerializeField]
    float radius;
    public float Value
    {
        get
        {
            return radius;
        }
    }
    public Task Set(float doublingValue, float radius)
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
        movingPattern.RoundingTracker(target.transform, doublingValue, radius);
    }
    public override State Run()
    {
        Debug.Log("Rounding");
        bool success = movingPattern.RoundingTracking();
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
        RoundingTrackAction parent = ScriptableObject.CreateInstance<RoundingTrackAction>();
        parent.Set(radius,doublingValue);
        return parent;
    }
}