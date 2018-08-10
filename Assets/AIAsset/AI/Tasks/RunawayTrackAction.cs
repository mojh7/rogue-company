using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/RunawayTrackAction")]
public class RunawayTrackAction : ActionTask
{

    MovingPattern movingPattern;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.target = RootTask.BlackBoard["Target"] as Character;
        movingPattern = character.GetCharacterComponents().AIController.MovingPattern;
        movingPattern.RunawayTracker(target.transform);
    }
    public override State Run()
    {
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

        return parent;
    }
}
