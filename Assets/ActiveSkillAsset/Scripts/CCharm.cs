using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Charm", menuName = "SkillData/Charm")]
public class CCharm : SkillData
{

    public override bool Run(Character character, object temporary)
    {
        return ActiveSkillManager.Instance.Charm(character, temporary, delay, amount);
    }
}

