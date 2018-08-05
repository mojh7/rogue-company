using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Confuse", menuName = "SkillData/Confuse")]
public class CConfuse : SkillData {

    public override BT.State Run(Character character, object temporary)
    {
        return ActiveSkillManager.Instance.Confuse(character, temporary, delay, amount);
    }
}
