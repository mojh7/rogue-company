using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [Header("For Object")]
    [SerializeField]
    protected float radius;
    [SerializeField]
    protected float amount;
    [SerializeField]
    protected float time;
    [SerializeField]
    protected float coolTime;
 
    public float Radius
    {
        get { return radius; }
    }
    public float Amount
    {
        get { return amount; }
    }
    public float CoolTime
    {
        get { return coolTime; }
    }

    protected Character caster;
    protected Character other;
    protected Vector3 mPos;
    protected CustomObject customObject;
    protected SkillObject skillObject;

    public virtual BT.State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime) // Object
    {
        if (lapsedTime < coolTime)
            return BT.State.FAILURE;
        lapsedTime = 0;
        this.customObject = customObject;
        this.mPos = pos;
        return BT.State.SUCCESS;
    }

    public virtual BT.State Run(Character caster, Vector3 pos, ref float lapsedTime) // Player
    {
        if (lapsedTime < coolTime)
            return BT.State.FAILURE;
        lapsedTime = 0;
        this.caster = caster;
        this.mPos = pos;
        return BT.State.SUCCESS;
    }

    public virtual BT.State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime) // Monster
    {
        if (lapsedTime < coolTime)
            return BT.State.FAILURE;
        lapsedTime = 0;
        this.caster = caster;
        this.other = other;
        this.mPos = pos;
        return BT.State.SUCCESS;
    }
}
