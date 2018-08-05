﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
[CreateAssetMenu(menuName = "Task/ShotAction")]
public class ShotAction : ActionTask {
    [SerializeField]
    int Idx;
    AttackPattern attackPattern;


    public Task Set(int Idx)
    {
        this.Idx = Idx;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        attackPattern = character.GetCharacterComponents().AIController.AttackPattern;
    }
    public override State Run()
    {
        bool success = attackPattern.Shot(character, 0);

        return State.SUCCESS;
    }
    public override Task Clone()
    {
        ShotAction parent = new ShotAction();

        return parent;
    }
}