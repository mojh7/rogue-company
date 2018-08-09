using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Charm", menuName = "SkillData/Charm")]
public class CCharm : SkillData
{

    public override BT.State Run(Character character, object temporary, int idx)
    {
        return ActiveSkillManager.Instance.Charm(character, temporary, idx, delay, amount);
    }
}

