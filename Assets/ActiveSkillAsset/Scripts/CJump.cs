using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CJump", menuName = "SkillData/CJump")]
public class CJump : SkillData
{
    public override bool Run(Character character, object temporary)
    {
        return ActiveSkillManager.Instance.Jump(character, temporary, delay, amount);
    }
}
