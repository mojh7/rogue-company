using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

[CreateAssetMenu(menuName = "Task/HealthDecorate")]
public class HealthDecorate : ConditionDecorate
{
    [SerializeField]
    float healthPer;
    public float Value
    {
        get
        {
            return healthPer;
        }
    }
    public Task SetValue(BehaviorCondition condition, float healthPer)
    {
        this.condition = condition;
        this.healthPer = healthPer;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;

    }
    public override State Run()
    {
        if (Check(character.GetPercentHP(), healthPer))
        {
            return GetChildren().Run();
        }
        else
        {
            return State.FAILURE;
        }
    }
    public override Task Clone()
    {
        HealthDecorate parent = new HealthDecorate();
        parent.SetValue(condition, healthPer);
        parent.Set(Probability);

        if (GetChildren() != null)
            parent.AddChild(GetChildren().Clone());

        return parent;
    }
}
