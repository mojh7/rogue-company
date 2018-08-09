    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HandUp", menuName = "SkillData/CHandUp")]
public class CHandUp : SkillData
{
    [SerializeField]
    float radius;
    [SerializeField]
    float num;

    public override BT.State Run(Character character,object temporary, int idx)
    {
        return ActiveSkillManager.Instance.HandUp(character, radius, idx, delay, amount, num);
    }
}
