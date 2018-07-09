using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 버프 류 생성 및 삭제할 때만 버프 내용 업데이트
// 사용되는 무기, 총알 쪽에서는 정보만 받아오기

// StartCoroutine 함수 monobehaviour 상속 받아야 됨.

// 버프, 패시브효과 관리

public class BuffManager : MonoBehaviour
{
    public enum TargetEffectTotalUpdateType { REGISTER, REMOVE }

    #region variables
    private List<CharacterTargetEffect> characterTargetEffects;
    private int characterTargetEffectsLength;
    private List<WeaponTargetEffect> weaponTargetEffects;
    private int weaponTargetEffectsLength;
    private Character owner;

    private CharacterTargetEffect characterTargetEffectTotal;
    private WeaponTargetEffect weaponTargetEffectTotal;
    #endregion

    #region get / set property
    public CharacterTargetEffect CharacterTargetEffectTotal
    {
        get { return characterTargetEffectTotal; }
    }
    public WeaponTargetEffect WeaponTargetEffectTotal
    {
        get { return weaponTargetEffectTotal; }
    }
    #endregion

    #region function
    public void Init()
    {
        characterTargetEffects = new List<CharacterTargetEffect>();
        weaponTargetEffects = new List<WeaponTargetEffect>();
        characterTargetEffectsLength = 0;
        weaponTargetEffectsLength = 0;
        InitCharacterTargetEffectTotal();
        InitWeaponTargetEffectTotal();
    }

    public void SetOwner(Character owner)
    {
        this.owner = owner;
    }

    /// <summary> 캐릭터 대상 효과 종합 초기화 </summary>
    public void InitCharacterTargetEffectTotal()
    {
        characterTargetEffectTotal = new CharacterTargetEffect
        {
            armorIncrease = 0,

            criticalChanceIncrease = 1f,
            moveSpeedIncrease = 1f,
            rewardOfEndGameIncrease = 1f,

            discountRateOfVendingMachineItems = 1f,
            discountRateOfCafeteriaItems = 1f,
            discountRateAllItems = 1f,

            hungerMaxIncrease = 1f,
            canDrainHp = false
        };
    }

    /// <summary> 무기 대상 효과 종합 초기화 </summary>
    public void InitWeaponTargetEffectTotal()
    {
        weaponTargetEffectTotal = new WeaponTargetEffect
        {
            shotgunBulletCountIncrease = 0,
            //criticalChanceIncrease = 0,

            damageIncrease = 1f,
            knockBackIncrease = 1f,
            chargeDamageIncrease = 1f,
            bulletScaleIncrease = 1f,
            bulletRangeIncrease = 1f,
            bulletSpeedIncrease = 1f,

            cooldownReduction = 1f,
            chargeTimeReduction = 1f,
            accuracyIncrease = 1f,
            shotgunsAccuracyIncrease = 1f,

            ammoCapacityIncrease = 0,
            //getTheHungerIncrease

            canIncreasePierceCount = false,
            becomesSpiderMine = false,
            bounceAble = false,
            shotgunBulletCanHoming = false,
            blowWeaponsCanBlockBullet = false,
            swingWeaponsCanReflectBullet = false
        };
    }


    /// <summary> 효과 등록 </summary>
    /// <param name="itemUseEffect">효과 내용</param>
    /// <param name="effectiveTime">효과 적용 시간, default = -1, 0초과된 값 => 일정 시간 동안 효과 적용되는 버프 아이템</param>
    public void RegisterItemEffect(ItemUseEffect itemUseEffect, float effectiveTime = -1f)
    {
        if (typeof(CharacterTargetEffect) == itemUseEffect.GetType())
        {
            CharacterTargetEffect targetEffect = itemUseEffect as CharacterTargetEffect;
            characterTargetEffects.Add(targetEffect);
            characterTargetEffectsLength += 1;

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

    /// <summary> 캐릭터 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(CharacterTargetEffect targetEffect)
    {
        characterTargetEffects.Remove(targetEffect);
        characterTargetEffectsLength -= 1;

        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }
    /// <summary> 무기 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(WeaponTargetEffect targetEffect)
    {
        weaponTargetEffects.Remove(targetEffect);
        weaponTargetEffectsLength -= 1;

        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }

    public void UpdateTargetEffectTotal(CharacterTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        int sign;
        bool boolSign;
        // 등록
        if (TargetEffectTotalUpdateType.REGISTER == updateType)
        {
            sign = 1;
            boolSign = true;
        }
        // 제거
        else
        {
            sign = -1;
            boolSign = false;
        }

        // CharacterTargetEffectTotal.recoveryHp += TargetEffect.recoveryHp;
        // CharacterTargetEffectTotal.recoveryHunger += TargetEffect.recoveryHunger;

        // 합 연산
        CharacterTargetEffectTotal.armorIncrease += targetEffect.armorIncrease * sign;

        // 곱 옵션 - 합 연산
        CharacterTargetEffectTotal.criticalChanceIncrease += targetEffect.criticalChanceIncrease * sign;
        CharacterTargetEffectTotal.moveSpeedIncrease += targetEffect.moveSpeedIncrease * sign;
        CharacterTargetEffectTotal.rewardOfEndGameIncrease += targetEffect.rewardOfEndGameIncrease * sign;
        //CharacterTargetEffectTotal += targetEffect * sign;

        // 곱 옵션 - 곱 연산
        if (1 == sign)
        {
            CharacterTargetEffectTotal.discountRateOfVendingMachineItems *= (1f - targetEffect.discountRateOfVendingMachineItems);
            CharacterTargetEffectTotal.discountRateOfCafeteriaItems *= (1f - targetEffect.discountRateOfCafeteriaItems);
            CharacterTargetEffectTotal.discountRateAllItems *= (1f - targetEffect.discountRateAllItems);
            //CharacterTargetEffectTotal *= (1.0f - targetEffect);
        }
        else
        {
            CharacterTargetEffectTotal.discountRateOfVendingMachineItems /= (1f - targetEffect.discountRateOfVendingMachineItems);
            CharacterTargetEffectTotal.discountRateOfCafeteriaItems /= (1f - targetEffect.discountRateOfCafeteriaItems);
            CharacterTargetEffectTotal.discountRateAllItems /= (1f - targetEffect.discountRateAllItems);
            //CharacterTargetEffectTotal /= (1.0f - targetEffect);
        }

        // 미정
        CharacterTargetEffectTotal.hungerMaxIncrease += targetEffect.hungerMaxIncrease * sign;

        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함.
        if (targetEffect.canDrainHp)
            CharacterTargetEffectTotal.canDrainHp = boolSign;

        PlayerManager.Instance.GetPlayer().UpdatePlayerData();
    }

    public void UpdateTargetEffectTotal(WeaponTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        int sign;
        bool boolSign;
        // 등록
        if (TargetEffectTotalUpdateType.REGISTER == updateType)
        {
            sign = 1;
            boolSign = true;
        }
        // 제거
        else
        {
            sign = -1;
            boolSign = false;
        }
        // 합 옵션
        weaponTargetEffectTotal.shotgunBulletCountIncrease += targetEffect.shotgunBulletCountIncrease * sign;
        //weaponTargetEffectTotal.criticalChanceIncrease += targetEffect.criticalChanceIncrease * sign;
        //weaponTargetEffectTotal += targetEffect * sign;        

        // 곱 옵션 - 합 연산
        weaponTargetEffectTotal.damageIncrease += targetEffect.damageIncrease * sign;
        weaponTargetEffectTotal.knockBackIncrease += targetEffect.knockBackIncrease * sign;
        weaponTargetEffectTotal.chargeDamageIncrease += targetEffect.chargeDamageIncrease * sign;
        weaponTargetEffectTotal.bulletScaleIncrease += targetEffect.bulletScaleIncrease * sign;
        weaponTargetEffectTotal.bulletRangeIncrease += targetEffect.bulletRangeIncrease * sign;
        weaponTargetEffectTotal.bulletSpeedIncrease += targetEffect.bulletSpeedIncrease * sign;

        // 곱 옵션 - 곱 연산
        if (1 == sign)
        {
            weaponTargetEffectTotal.cooldownReduction *= (1.0f - targetEffect.cooldownReduction);
            weaponTargetEffectTotal.chargeTimeReduction *= (1.0f - targetEffect.chargeTimeReduction);
            weaponTargetEffectTotal.accuracyIncrease *= (1.0f - targetEffect.accuracyIncrease);
            weaponTargetEffectTotal.shotgunsAccuracyIncrease = (1.0f - targetEffect.shotgunsAccuracyIncrease);
            //weaponTargetEffectTotal *= (1.0f - targetEffect);
        }
        else
        {
            weaponTargetEffectTotal.cooldownReduction /= (1.0f - targetEffect.cooldownReduction);
            weaponTargetEffectTotal.chargeTimeReduction /= (1.0f - targetEffect.chargeTimeReduction);
            weaponTargetEffectTotal.accuracyIncrease /= (1.0f - targetEffect.accuracyIncrease);
            weaponTargetEffectTotal.shotgunsAccuracyIncrease /= (1.0f - targetEffect.shotgunsAccuracyIncrease);
            //weaponTargetEffectTotal /= (1.0f - targetEffect);
        }

        // 미정
        weaponTargetEffectTotal.ammoCapacityIncrease += targetEffect.ammoCapacityIncrease * sign;
        weaponTargetEffectTotal.getTheHungerIncrease += targetEffect.getTheHungerIncrease * sign;

        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함. 
        if (targetEffect.canIncreasePierceCount)
            weaponTargetEffectTotal.canIncreasePierceCount = boolSign;
        if (targetEffect.becomesSpiderMine)
            weaponTargetEffectTotal.becomesSpiderMine = boolSign;
        if (targetEffect.bounceAble)
            weaponTargetEffectTotal.bounceAble = boolSign;
        if (targetEffect.blowWeaponsCanBlockBullet)
            weaponTargetEffectTotal.blowWeaponsCanBlockBullet = boolSign;
        if (targetEffect.swingWeaponsCanReflectBullet)
            weaponTargetEffectTotal.swingWeaponsCanReflectBullet = boolSign;
    }


    #endregion

    private IEnumerator RemoveTargetEffectOnEffectiveTime(CharacterTargetEffect targetEffect, float effectiveTime)
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
