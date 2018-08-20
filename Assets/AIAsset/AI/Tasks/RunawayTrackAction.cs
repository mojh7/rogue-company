using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/RunawayTrackAction")]
public class RunawayTrackAction : ActionTask
{
    [SerializeField]
    float doublingValue;
    MovingPattern movingPattern;
    public Task Set(float doublingValue)
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
        movingPattern.RunawayTracker(target.transform, doublingValue);
    }
    public override State Run()
    {
        if (character.isCasting)
            return State.FAILURE;
        character.SetAutoAim();
        bool success = movingPattern.RunawayTracking();
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
        RunawayTrackAction parent = ScriptableObject.CreateInstance<RunawayTrackAction>();
        parent.Set(doublingValue);
        return parent;
    }
}
