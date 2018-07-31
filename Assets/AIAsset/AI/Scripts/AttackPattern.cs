using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : MonoBehaviour
{
    #region variables
    bool isActive;
    #endregion
    SkillData[] skillDatas;

    #region Func
    public void Init(SkillData[] skillDatas)
    {
        this.skillDatas = skillDatas;
        this.isActive = true;
    }
    public void Play()
    {
        isActive = true;
    }
    public void Stop()
    {
        if (!isActive)
        {
            return;
        }
        isActive = false;
    }
    #endregion
    public void Temp(Character character, float time, float amount, float radiuse, float num)
    {
        ActiveSkillManager.Instance.HandClap(character, radiuse, time, amount);
    }

    public void CastingSpell(Character character, int i, object temporary, params float[] param)
    {
        if (i >= skillDatas.Length || !isActive)
            return;
        skillDatas[i].Run(character, temporary, param);    
    }
}
