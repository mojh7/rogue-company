using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/SubSelector")]
public class SubSelector : CompositeTask {

    public override bool Run()
    {
        int i = 0;
        foreach (var task in GetChildren())
        {
            i++;
            if(i == 7)
            {
                Debug.Log("A");
            }
            if (task.Run())
            {
                Debug.Log(i);
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
