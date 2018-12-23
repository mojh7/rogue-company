using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CDelayImporter", menuName = "SkillData/CDelayImporter")]
public class CDelayImporter : SkillData
{
    [SerializeField]
    List<SkillData> skillData;

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
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }

        foreach (SkillData item in skillData)
        {
            ActiveSkillManager.Instance.DelaySkill(mPos, item, caster, other, customObject, time);
        }

        return BT.State.SUCCESS;
    }
}