using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [SerializeField]
    protected float delay;
    [SerializeField]
    protected float amount;

    public abstract bool Run(Character character, object temporary);
}
