using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        idx--;
        return ActiveSkillManager.Instance.SpawnServant(character, temporary, idx, delay, amount);
    }
}
