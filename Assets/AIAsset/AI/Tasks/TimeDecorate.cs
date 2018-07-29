using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class TimeDecorate : ConditionDecorate
{

    Character target;
    [SerializeField]
    float time;
    float startTime;
    float elapsedTime;
    bool isRun;
    public Task Set(BehaviorCondition condition, float time)
    {
        this.condition = condition;
        this.time = time;
        this.isRun = false;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
    }
    public override bool Run()
    {
        if (!isRun)
        {
            isRun = true;
            startTime = Time.time;
        }
        elapsedTime = Time.time - startTime;

        if (Check(elapsedTime, time))
        {
            return GetChildren().Run();
        }
        else
        {
            return false;
        }
    }
    public override bool SubRun()
    {
        startTime = Time.deltaTime;
        elapsedTime = 0;
        isRun = false;
        return true;
    }
    public override Task Clone()
    {
        TimeDecorate parent = new TimeDecorate();
        parent.Set(condition, time);
        parent.AddChild(GetChildren().Clone());

        return parent;
    }
}
