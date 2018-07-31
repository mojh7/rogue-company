using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : MonoBehaviour
{
    SkillData[] skillDatas;

    public void Init(SkillData[] skillDatas)
    {
        this.skillDatas = skillDatas;
    }
    public void Temp(Character character, float time, float amount, float radiuse, float num)
    {
        ActiveSkillManager.Instance.HandUp(character, time, amount, radiuse, num);
    }

    public void Skill(Character character, int i, params float[] param)
    {
        if (i >= skillDatas.Length)
            return;
        skillDatas[i].Run(character, param);    
    }
}
