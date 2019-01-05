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
    float[] lapsedTimes;
    int skillLength;
    #region Func
    public void Init(SkillData[] skillDatas,WeaponManager weaponManager)
    {
        this.skillDatas = skillDatas;
        this.skillLength = skillDatas.Length;
        this.lapsedTimes = new float[skillLength];
        this.weaponManager = weaponManager;
        this.isActive = true;
        for(int i=0;i< skillLength;i++)
        {
            lapsedTimes[i] = 9999;
        }
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
    private void Update()
    {
        for (int i = 0; i < skillLength; i++)
        {
            lapsedTimes[i] += Time.deltaTime;
        }

    }
    #endregion

    public bool Shot(Character character, int i)
    {
        weaponManager.AttackWeapon(i);
        return false;  
    }

    public BT.State CastingSKill(Character caster, Character other, int i)
    {
        if (i >= skillDatas.Length || !isActive)
            return BT.State.FAILURE;
        return skillDatas[i].Run(caster, other, ActiveSkillManager.nullVector,ref lapsedTimes[i]);
    }
}
