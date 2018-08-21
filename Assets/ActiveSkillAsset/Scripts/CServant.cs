using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        return ActiveSkillManager.Instance.SpawnServant(character, (character as Enemy).GetServants(), idx, delay, amount);
    }
}
