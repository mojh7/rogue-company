using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CRepeater", menuName = "SkillData/CRepeater")]
public class CRepeater : SkillData
{
    [SerializeField]
    List<SkillData> skillData;

    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        for (int i = 0; i < amount; i++)
        {
            foreach (SkillData item in skillData)
            {
                item.Run(customObject, pos, ref lapsedTime);
            }

        }
        return BT.State.SUCCESS;
    }

    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        for (int i = 0; i < amount; i++)
        {
            foreach (SkillData item in skillData)
            {
                item.Run(caster, pos, ref lapsedTime);
            }

        }
        return BT.State.SUCCESS;
    }

    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;

        for(int i=0;i<amount;i++)
        {
            float rTime = Random.Range(0, time);
            foreach (SkillData item in skillData)
            {
                ActiveSkillManager.Instance.DelaySkill(mPos, item, caster, other, customObject, rTime);
            }
        }
        return BT.State.SUCCESS;
    }

}
