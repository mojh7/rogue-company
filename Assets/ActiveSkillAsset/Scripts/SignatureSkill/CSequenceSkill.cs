using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CSequenceSkill", menuName = "SkillData/CSequenceSkill")]
public class CSequenceSkill : SkillData
{
    [Space]

    [SerializeField]
    SkillData preSkillData;
    [SerializeField]
    SkillData mainSkillData;
    [SerializeField]
    SkillData postSkillData;

    [SerializeField]
    int frontAnimIdx = -1, backAnimIdx = -1;
    [SerializeField]
    float frontAnimTime = 0, backAnimTime = 0;

    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(lapsedTime);
    }
    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(lapsedTime);
    }
    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(lapsedTime);
    }

    private BT.State Run(float lapsedTime)
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }

        ActiveSkillManager.Instance.StartSequence(mPos,
            caster, other, customObject,
            preSkillData, mainSkillData, postSkillData,
            frontAnimIdx, backAnimIdx, frontAnimTime, backAnimTime);
        return BT.State.SUCCESS;
    }


}