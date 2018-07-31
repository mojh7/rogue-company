using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandClap", menuName = "SkillData/CHandClap", order = 1)]
public class CHandClap : SkillData
{
    public override bool Run(Character character, object temporary)
    {
        return ActiveSkillManager.Instance.HandClap(character, temporary, delay, amount);
    }
}
