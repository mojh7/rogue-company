using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBox : BreakalbeBox
{
    [SerializeField]
    ObjectAbnormalType objectAbnormalType;

    public override void Init()
    {
        base.Init();
        objectType = ObjectType.SKILLBOX;
    }

    public void Init(ObjectAbnormalType objectAbnormal)
    {
        Init();
        objectAbnormalType = objectAbnormal;
    }

    protected override void Destruct()
    {
        SkillData skillData = null;
        switch (objectAbnormalType)
        {
            case ObjectAbnormalType.FREEZE:
                skillData = ObjectSkillManager.Instance.GetSkillData("Freeze");
                break;
            case ObjectAbnormalType.POISON:
                skillData = ObjectSkillManager.Instance.GetSkillData("Poison");
                break;
            case ObjectAbnormalType.BURN:
                skillData = ObjectSkillManager.Instance.GetSkillData("Burn");
                break;
            case ObjectAbnormalType.STUN:
                skillData = ObjectSkillManager.Instance.GetSkillData("Stun");
                break;
            case ObjectAbnormalType.CHARM:
                skillData = ObjectSkillManager.Instance.GetSkillData("Charm");
                break;
            default:
                break;
        }
        float lapsedTime = 9999;
        if (skillData != null)
            skillData.Run(this, ActiveSkillManager.nullVector, ref lapsedTime);
        base.Destruct();
    }
}
