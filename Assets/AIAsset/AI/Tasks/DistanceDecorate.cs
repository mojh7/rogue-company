using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 거리 조건에 따라 자식의 수행 여부를 정하는 조건 노드입니다.
/// </summary>
public class DistanceDecorate : ConditionDecorate
{
    Character target;
    [SerializeField]
    float distance;
    public Task Set(BehaviorCondition condition, float distance)
    {
        this.condition = condition;
        this.distance = distance;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.target = RootTask.BlackBoard["Target"] as Character;
    }
    public override bool Run()
    {
        if (Check(Vector2.Distance(character.transform.position, target.transform.position), distance))
        {
            return GetChildren().Run();
        }
        else
        {
            return false;
        }
    }
    public override Task Clone()
    {
        DistanceDecorate parent = new DistanceDecorate();
        parent.Set(condition, distance);
        parent.AddChild(GetChildren().Clone());

        return parent;
    }
}