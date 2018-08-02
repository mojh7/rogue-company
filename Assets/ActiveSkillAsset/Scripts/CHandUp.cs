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

    public override bool Run(Character character,object temporary)
    {
        return ActiveSkillManager.Instance.HandUp(character, radius, delay, amount, num);
    }
}
