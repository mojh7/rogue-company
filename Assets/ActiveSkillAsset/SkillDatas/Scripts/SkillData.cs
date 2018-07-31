using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected float delay;

    public abstract bool Run(Character character, object temporary, params float[] param);
}
