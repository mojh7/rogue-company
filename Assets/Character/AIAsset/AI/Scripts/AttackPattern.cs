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
    public void Init(SkillData[] skillDatas,WeaponManager weaponManager)
    {
        this.skillDatas = skillDatas;
        this.weaponManager = weaponManager;
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
        weaponManager.AttackWeapon(i);
        return false;  
    }

    public BT.State CastingSKill(Character character, object temporary, int i)
    {
        if (i >= skillDatas.Length || !isActive)
            return BT.State.FAILURE;
        return skillDatas[i].Run(character, temporary, i);
    }
}
