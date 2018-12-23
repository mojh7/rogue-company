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

    public float Radius
    {
        get { return radius; }
    }
    public float Amount
    {
        get { return amount; }
    }

    protected Character caster;
    protected Character other;
    protected Vector3 mPos;
    protected CustomObject customObject;
    protected SkillObject skillObject;

    public virtual BT.State Run(CustomObject customObject, Vector3 pos) // Object
    {
        this.customObject = customObject;
        this.mPos = pos;
        return BT.State.SUCCESS;
    }

    public virtual BT.State Run(Character caster, Vector3 pos) // Player
    {
        this.caster = caster;
        this.mPos = pos;
        return BT.State.SUCCESS;
    }

    public virtual BT.State Run(Character caster, Character other, Vector3 pos) // Monster
    {
        this.caster = caster;
        this.other = other;
        this.mPos = pos;
        return BT.State.SUCCESS;
    }
}
