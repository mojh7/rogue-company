using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/BoolDecorate")]
public class BoolDecorate : DecorateTask
{
    System.Func<bool> func;
    AIController aIController;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        aIController = this.character.GetCharacterComponents().AIController;
        func = delegate () { return character.GetAIAct(); };
    }
    public override bool Run()
    {
        if (func())
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
        BoolDecorate parent = ScriptableObject.CreateInstance<BoolDecorate>();
        if (GetChildren() != null)
            parent.AddChild(GetChildren().Clone());

        return parent;
    }
}