    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HandUp", menuName = "SkillData/CHandUp", order = 0)]
public class CHandUp : SkillData
{
    public override bool Run(Character character,object temporary,params float[] param)
    {
        return ActiveSkillManager.Instance.HandUp(character, temporary, param[0], param[1], param[2]);
    }
}
