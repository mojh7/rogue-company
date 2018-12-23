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

    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run();
    }
    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run();
    }
    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run();
    }

    private BT.State Run()
    {
        if (delay < 0 || amount < 0)
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