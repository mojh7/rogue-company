using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/BoolDecorate")]
public class BoolDecorate : DecorateTask
{
    System.Func<bool> func;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        func = delegate () { return character.GetAIAct(); };
    }
    public override State Run()
    {
        if (func())
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
        BoolDecorate parent = ScriptableObject.CreateInstance<BoolDecorate>();
        parent.Set(Probability);

        if (GetChildren() != null)
            parent.AddChild(GetChildren().Clone());

        return parent;
    }
}