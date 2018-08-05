using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/SubSelector")]
public class SubSelector : CompositeTask {

    public override State Run()
    {
        foreach (var task in GetChildren())
        {
            State state = task.Run();
            if (state == State.SUCCESS || state == State.CONTINUE)
            {
                return state;
            }
        }
        foreach (var task in GetChildren())
        {
            task.SubRun();
        }
        return State.FAILURE;
    }
    public override Task Clone()
    {
        SubSelector parent = new SubSelector();
        if (GetChildren() != null)
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
        return parent;
    }
}
