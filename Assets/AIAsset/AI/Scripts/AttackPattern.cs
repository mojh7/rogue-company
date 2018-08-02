using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : MonoBehaviour
{
    #region components
    WeaponManager weaponManager;
    #endregion
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
  
    public bool Shot(Character character, int i)
    {
        return false;  
    }

    public bool CastingSKill(Character character, int i, object temporary)
    {
        if (i >= skillDatas.Length || !isActive)
            return false;
        skillDatas[i].Run(character, temporary);
        return true;
    }
}
