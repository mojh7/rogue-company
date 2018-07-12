﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class BoolDecorate : DecorateTask
{
    System.Func<bool> func;

    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
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
        BoolDecorate parent = new BoolDecorate();
        parent.AddChild(GetChildren().Clone());

        return parent;
    }
}