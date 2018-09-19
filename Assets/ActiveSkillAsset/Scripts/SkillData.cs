using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [SerializeField]
    protected float delay;
    [SerializeField]
    protected float amount;
    protected Character character;
    protected object temporary;
    protected int idx;
    public virtual BT.State Run(Character character, object temporary, int idx)
    {
        this.character = character;
        this.temporary = temporary;
        this.idx = idx;
        return BT.State.SUCCESS;
    }
}
