using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandClap", menuName = "SkillData/CHandClap", order = 1)]
public class CHandClap : SkillData
{
    public override bool Run(Character character, object temporary, params float[] param)
    {
        return ActiveSkillManager.Instance.HandClap(character, temporary, param[0], param[1]);
    }
}
