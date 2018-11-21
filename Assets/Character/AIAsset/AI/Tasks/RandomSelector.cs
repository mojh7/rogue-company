using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

[CreateAssetMenu(menuName = "Task/RandomSelector")]
public class RandomSelector : CompositeTask
{
    float total;
    bool isRun;
    float randomPoint;
    float tempRandomPoint;
    public override void Init(Task task)
    {
        base.Init(task);
        isRun = false;
        foreach (var child in GetChildren())
        {
            total += child.Probability;
        }
    }

    public override State Run()
    {
        if (!isRun)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            randomPoint = Random.value * total;
            isRun = true;
        }
        tempRandomPoint = randomPoint;

        foreach (var task in GetChildren())
        {
            if(tempRandomPoint < task.Probability)
            {
                State state = task.Run();
                if (state == State.SUCCESS || state == State.CONTINUE)
                {
                    return state;
                }
                tempRandomPoint += task.Probability;
            }
            else
            {
                tempRandomPoint -= task.Probability;
            }

        }

        isRun = false;

        return State.SUCCESS;
    }

    public override Task Clone()
    {
        RandomSelector parent = ScriptableObject.CreateInstance<RandomSelector>();
        parent.Set(Probability);
        if (GetChildren() != null)
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
        return parent;
    }
}
