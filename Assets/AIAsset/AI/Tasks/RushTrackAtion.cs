using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 기본 A* 돌진 행동을 담은 노드입니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/RushTrackAtion")]
public class RushTrackAtion : ActionTask
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
        movingPattern.RushTracker(target.transform, doublingValue);
    }
    public override State Run()
    {
        if (character.isCasting)
            return State.FAILURE;
        character.SetAimType(CharacterInfo.AimType.AUTO);
        bool success = movingPattern.RushTracking();
        if (success)
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
        RushTrackAtion parent = ScriptableObject.CreateInstance<RushTrackAtion>();
        parent.Set(Probability);
        parent.SetValue(doublingValue);
        return parent;
    }
}