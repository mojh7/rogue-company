using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CFlash", menuName = "SkillData/CFlash")]
public class CFlash : SkillData
{
    public override bool Run(Character character, object temporary)
    {
        return ActiveSkillManager.Instance.Flash(character, temporary, delay, amount);
    }
}
