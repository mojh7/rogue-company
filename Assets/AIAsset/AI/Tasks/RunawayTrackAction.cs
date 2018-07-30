using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

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
    public override bool Run()
    {
        success = movingPattern.RunawayTracking();
        Debug.Log("Runaway");
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
        RunawayTrackAction parent = new RunawayTrackAction();

        return parent;
    }
}
