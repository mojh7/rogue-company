using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/SubSelector")]
public class SubSelector : CompositeTask {

    public override bool Run()
    {
        foreach (var task in GetChildren())
        {
            if (task.Run())
            {
                return true;
            }
        }
        foreach (var task in GetChildren())
        {
            task.SubRun();
        }
        return false;
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
