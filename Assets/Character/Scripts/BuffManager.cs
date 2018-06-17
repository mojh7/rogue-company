using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 예전에 쓴 것들 바꿔야됨.



/* Onwer 안에 속해서 각종 TargetEffect를 관리
 * 
 * 패시브 아이템, 시간제 버프 아이템이 Onwer에게 생성되고 발생되면 여기에 등록되었다가
 * 삭제 될 때
 *  - 시간제 버프 아이템은 적용 시간 다 될 때
 *  - 게임이 완전이 끝나서 초기화 해야될 때 패시브 아이템 삭제
 *  - 무언가 상위 아이템으로 바뀌게 되서 기존의 아이템 삭제 해야 될 때
 * 여기에 등록 되었던 아이템 삭제
 * 
 * 
 * 버프 종류
 *  # 단순 능력치
 *   - 공격력 상승률, 치명타 상승률, 총알 사정거리 상승률 등등 float형이고 공격력 + 20% => +0.2f 식으로
 *     TargetEffectTotal에 더해져서 1.0f + 0.2f = 1.2f;
 *   - 버프 추가시에만 더하기 연산 하고 실제로 쓰는 weapon, bullet에서는 값만 읽어와서 기존 공격력에 값만 곱해줘서 사용하면 됨.
 *   
 *  # 추가 속성
 *   - ex : 레이저 무기 빙결 속성 추가 같은 특정 조건을 만족하는 무기에 대한 특정 속성 추가같은 경우
 *   - 추가후에 삭제시에 알맞는 거에 대한 삭제를 하려면 추가적인 조취를 해줘야 되서
 *     weapon, bullet 사용시에 weaponTargetEffects를 모두 순회하며 추가해야 될 property를 즉각적으로 읽어와서 추가하여 사용하면 되지 않을까 함.
 *     
 *  
 */


// 버프 류 생성 및 삭제할 때만 버프 내용 업데이트
// 사용되는 무기, 총알 쪽에서는 정보만 받아오기


// 버프, 패시브효과 관리

public class BuffManager : MonoBehaviourSingleton<BuffManager>
{
    public enum TargetEffectTotalUpdateType { REGISTER, REMOVE }

    #region variables
    private List<PlayerTargetEffect> playerTargetEffects;
    private int playerTargetEffectsLength;
    private List<WeaponTargetEffect> weaponTargetEffects;
    private int weaponTargetEffectsLength;

    private PlayerTargetEffect playerTargetEffectTotal;
    private WeaponTargetEffect weaponTargetEffectTotal;
    #endregion

    #region get / set property
    public PlayerTargetEffect PlayerTargetEffectTotal
    {
        get { return playerTargetEffectTotal; }
    }
    public WeaponTargetEffect WeaponTargetEffectTotal
    {
        get { return weaponTargetEffectTotal; }
    }
    #endregion

    #region unityFunc
    private void Awake()
    {
        playerTargetEffects = new List<PlayerTargetEffect>();
        weaponTargetEffects = new List<WeaponTargetEffect>();
        playerTargetEffectsLength = 0;
        weaponTargetEffectsLength = 0;
        InitPlayerTargetEffectTotal();
        InitWeaponTargetEffectTotal();
    }

    #endregion

    #region function

    public void InitPlayerTargetEffectTotal()
    {
        playerTargetEffectTotal = new PlayerTargetEffect
        {
            hungerMaxIncrease = 1f,
            moveSpeedIncrease = 1f,
            criticalChanceIncrease = 1f,
            armorIncrease = 0
        };
    }
    

    public void InitWeaponTargetEffectTotal()
    {
        weaponTargetEffectTotal = new WeaponTargetEffect
        {
            cooldownReduction = 1f,
            damageIncrease = 1f,
            knockBackIncrease = 1f,
            bulletScaleIncrease = 1f,
            bulletRangeIncrease = 1f,
            bulletSpeedIncrease = 1f,
            chargeTimeReduction = 1f,
            chargeDamageIncrease = 1f,

            ammoCapacityIncrease = 0,
            shotgunBulletCountIncrease = 0
        };
    }


    // effectiveTime = 효과 적용시간 
    // default = -1 => 패시브
    // 0초과된 값 => 일정 시간 동안 효과 적용되는 버프 아이템
    public void RegisterItemEffect(ItemUseEffect itemUseEffect, float effectiveTime = -1f)
    {
        
        if (typeof(PlayerTargetEffect) == itemUseEffect.GetType())
        {
            PlayerTargetEffect targetEffect = itemUseEffect as PlayerTargetEffect;
            playerTargetEffects.Add(targetEffect);
            playerTargetEffectsLength += 1;

            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
                StartCoroutine(RemoveTargetEffectOnEffectiveTime(targetEffect, effectiveTime));
        }
        else
        {
            WeaponTargetEffect targetEffect = itemUseEffect as WeaponTargetEffect;
            weaponTargetEffects.Add(targetEffect);
            weaponTargetEffectsLength += 1;

            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
                StartCoroutine(RemoveTargetEffectOnEffectiveTime(targetEffect, effectiveTime));
        }
        
    }

    // 버프 제거
    public void RemoveTargetEffect(PlayerTargetEffect targetEffect)
    {
        playerTargetEffects.Remove(targetEffect);
        playerTargetEffectsLength -= 1;

        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }
    // 버프 제거
    public void RemoveTargetEffect(WeaponTargetEffect targetEffect)
    {
        weaponTargetEffects.Remove(targetEffect);
        weaponTargetEffectsLength -= 1;

        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }

    public void UpdateTargetEffectTotal(PlayerTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        int sign;
        if (TargetEffectTotalUpdateType.REGISTER == updateType)
            sign = 1;
        else
            sign = -1;

        // playerTargetEffectTotal.recoveryHp += TargetEffect.recoveryHp;
        // playerTargetEffectTotal.recoveryHunger += TargetEffect.recoveryHunger;
        playerTargetEffectTotal.moveSpeedIncrease += targetEffect.moveSpeedIncrease * sign;
        playerTargetEffectTotal.hungerMaxIncrease += targetEffect.hungerMaxIncrease * sign;
        playerTargetEffectTotal.armorIncrease += targetEffect.armorIncrease * sign;
        playerTargetEffectTotal.criticalChanceIncrease += targetEffect.criticalChanceIncrease * sign;

        PlayerManager.Instance.GetPlayer().UpdatePlayerData();
    }

    public void UpdateTargetEffectTotal(WeaponTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        int sign;
        if (TargetEffectTotalUpdateType.REGISTER == updateType)
            sign = 1;
        else
            sign = -1;

        weaponTargetEffectTotal.cooldownReduction += targetEffect.cooldownReduction * sign;
        weaponTargetEffectTotal.damageIncrease += targetEffect.damageIncrease * sign;
        weaponTargetEffectTotal.criticalChanceIncrease += targetEffect.criticalChanceIncrease * sign;
        weaponTargetEffectTotal.knockBackIncrease += targetEffect.knockBackIncrease * sign;
        weaponTargetEffectTotal.ammoCapacityIncrease += targetEffect.ammoCapacityIncrease * sign;

        weaponTargetEffectTotal.bulletScaleIncrease += targetEffect.bulletScaleIncrease * sign;
        weaponTargetEffectTotal.bulletRangeIncrease += targetEffect.bulletRangeIncrease * sign;
        weaponTargetEffectTotal.bulletSpeedIncrease += targetEffect.bulletSpeedIncrease * sign;

        weaponTargetEffectTotal.chargeTimeReduction += targetEffect.chargeTimeReduction * sign;
        weaponTargetEffectTotal.chargeDamageIncrease += targetEffect.chargeDamageIncrease * sign;
        weaponTargetEffectTotal.shotgunBulletCountIncrease += targetEffect.shotgunBulletCountIncrease * sign;
    }


    #endregion

    private IEnumerator RemoveTargetEffectOnEffectiveTime(PlayerTargetEffect targetEffect, float effectiveTime)
    {
        float time = 0;
        while (true)
        {
            if (time >= effectiveTime)
            {
                RemoveTargetEffect(targetEffect);
            }
            time += Time.fixedTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedTime);
        }
    }

    private IEnumerator RemoveTargetEffectOnEffectiveTime(WeaponTargetEffect targetEffect, float effectiveTime)
    {
        float time = 0;
        while (true)
        {
            if (time >= effectiveTime)
            {
                RemoveTargetEffect(targetEffect);
            }
            time += Time.fixedTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedTime);
        }
    }
}
