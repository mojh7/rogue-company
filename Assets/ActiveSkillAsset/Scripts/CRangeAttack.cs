using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CRangeAttack", menuName = "SkillData/CRangeAttack")]
public class CRangeAttack : SkillData
{
    public override BT.State Run(Character character, object temporary)
    {
        return ActiveSkillManager.Instance.RangeAttack(character, temporary, delay, amount);
    }
}
