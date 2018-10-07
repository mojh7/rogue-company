using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/TimeDecorate")]
public class TimeDecorate : ConditionDecorate
{

    Character target;
    [SerializeField]
    float time;
    float startTime;
    float elapsedTime;
    bool isRun;

    public float Value
    {
        get
        {
            return time;
        }
    }

    public Task Set(BehaviorCondition condition, float time)
    {
        this.condition = condition;
        this.time = time;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.isRun = false;
    }
    public override State Run()
    {
        if (!isRun)
        {
            isRun = true;
            startTime = Time.time;
            return GetChildren().Run();
        }
        else
        {
            elapsedTime = Time.time - startTime;

            if (Check(elapsedTime, time))
            {
                if(condition == BehaviorCondition.COOLTIME)
                {
                    isRun = false;
                    GetChildren().SubRun();
                }
                return GetChildren().Run();
            }
            else
            {
                return State.FAILURE;
            }
        }
    }
    public override bool SubRun()
    {
        isRun = false;
        GetChildren().SubRun();
        return true;
    }
    public override Task Clone()
    {
        TimeDecorate parent = ScriptableObject.CreateInstance<TimeDecorate>();
        parent.Set(Probability);
        parent.Set(condition, time);
        if(GetChildren() != null)
            parent.AddChild(GetChildren().Clone());

        return parent;
    }
}
