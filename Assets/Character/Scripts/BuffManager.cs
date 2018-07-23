using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 버프 류 생성 및 삭제할 때만 버프 내용 업데이트
// owner 관련 효과는 종합 정보 가지고 있다가 owner쪽 함수 실행하고 종합 정보 값 넘겨서 업데이트 하기
// 무기 관련 효과는 종합 정보 가지고 있다가 사용되는 무기, 총알 쪽에서 buffManager를 접근해서 사용하기

// StartCoroutine 함수 monobehaviour 상속 받아야 됨.
// 버프, 패시브효과 관리

public class BuffManager : MonoBehaviour
{
    public enum TargetEffectTotalUpdateType { REGISTER, REMOVE }
    public enum EffectApplyType { BUFF, PASSIVE }

    #region variables
    private List<ItemUseEffect> passiveEffects;
    private int passiveEffectsLength;
    private List<ItemUseEffect> buffEffects;
    private int buffEffectsLength;

    //private List<CharacterTargetEffect> characterTargetEffects;
    //private int characterTargetEffectsLength;
    //private List<WeaponTargetEffect> weaponTargetEffects;
    //private int weaponTargetEffectsLength;
    private Character owner;

    private CharacterTargetEffect characterTargetEffectTotal;
    private WeaponTargetEffect weaponTargetEffectTotal;
    #endregion

    #region get / set property
    public List<ItemUseEffect> PassiveEffects
    {
        get { return passiveEffects; }
    }
    public int PassiveEffectsLength
    {
        get { return passiveEffectsLength; }
    }
    public List<ItemUseEffect> BuffEffects
    {
        get { return buffEffects; }
    }
    public int BuffEffectsLength
    {
        get { return buffEffectsLength; }
    }
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
        passiveEffects = new List<ItemUseEffect>();
        buffEffects = new List<ItemUseEffect>();
        //characterTargetEffects = new List<CharacterTargetEffect>();
        //weaponTargetEffects = new List<WeaponTargetEffect>();
        passiveEffectsLength = 0;
        buffEffectsLength = 0;
        //characterTargetEffectsLength = 0;
        //weaponTargetEffectsLength = 0;
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
            armorIncrement = 0,

            criticalChanceIncrement = 1f,
            moveSpeedIncrement = 1f,
            rewardOfEndGameIncrement = 1f,

            discountRateOfVendingMachineItems = 1f,
            discountRateOfCafeteriaItems = 1f,
            discountRateAllItems = 1f,

            hungerMaxIncrement = 1f,
            canDrainHp = false
        };
    }

    /// <summary> 무기 대상 효과 종합 초기화 </summary>
    public void InitWeaponTargetEffectTotal()
    {
        weaponTargetEffectTotal = new WeaponTargetEffect
        {
            shotgunBulletCountIncrement = 0,
            //criticalChanceIncrement = 0,

            damageIncrement = 1f,
            knockBackIncrement = 1f,
            chargingAmountIncrement = 1f,
            gettingSkillGaugeIncrement = 1f,
            gettingStaminaIncrement = 1f,
            skillPowerIncrement = 1f, 
            bulletScaleIncrement = 1f,
            bulletRangeIncrement = 1f,
            bulletSpeedIncrement = 1f,

            cooldownReduction = 1f,
            chargeTimeReduction = 1f,
            accuracyIncrement = 1f,
            shotgunsAccuracyIncrement = 1f,

            ammoCapacityIncrement = 0,
            //getTheHungerIncrement

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
        if (-1 == effectiveTime)
        {
            passiveEffects.Add(itemUseEffect);
            passiveEffectsLength += 1;
        }
        else
        {
            buffEffects.Add(itemUseEffect);
            buffEffectsLength += 1;
        }

        if (typeof(CharacterTargetEffect) == itemUseEffect.GetType())
        {
            CharacterTargetEffect targetEffect = itemUseEffect as CharacterTargetEffect;
            //characterTargetEffects.Add(targetEffect);
            //characterTargetEffectsLength += 1;
            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
                StartCoroutine(RemoveTargetEffectOnEffectiveTime(targetEffect, effectiveTime));
        }
        else
        {
            WeaponTargetEffect targetEffect = itemUseEffect as WeaponTargetEffect;
            //weaponTargetEffects.Add(targetEffect);
            //weaponTargetEffectsLength += 1;
            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
                StartCoroutine(RemoveTargetEffectOnEffectiveTime(targetEffect, effectiveTime));
        }
        
    }

    /// <summary> 캐릭터 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(CharacterTargetEffect targetEffect, EffectApplyType effectApplyType)
    {
        if (EffectApplyType.BUFF == effectApplyType)
        {
            passiveEffects.Remove(targetEffect);
            passiveEffectsLength -= 1;
        }
        else
        {
            buffEffects.Remove(targetEffect);
            buffEffectsLength -= 1;
        }
        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }
    /// <summary> 무기 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(WeaponTargetEffect targetEffect, EffectApplyType effectApplyType)
    {
        if (EffectApplyType.BUFF == effectApplyType)
        {
            passiveEffects.Remove(targetEffect);
            passiveEffectsLength -= 1;
        }
        else
        {
            buffEffects.Remove(targetEffect);
            buffEffectsLength -= 1;
        }
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
        CharacterTargetEffectTotal.armorIncrement += targetEffect.armorIncrement * sign;

        // 곱 옵션 - 합 연산
        CharacterTargetEffectTotal.criticalChanceIncrement += targetEffect.criticalChanceIncrement * sign;
        CharacterTargetEffectTotal.moveSpeedIncrement += targetEffect.moveSpeedIncrement * sign;
        CharacterTargetEffectTotal.rewardOfEndGameIncrement += targetEffect.rewardOfEndGameIncrement * sign;
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
        CharacterTargetEffectTotal.hungerMaxIncrement += targetEffect.hungerMaxIncrement * sign;

        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함.
        if (targetEffect.canDrainHp)
            CharacterTargetEffectTotal.canDrainHp = boolSign;

        owner.ApplyItemEffect(characterTargetEffectTotal);
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
        weaponTargetEffectTotal.shotgunBulletCountIncrement += targetEffect.shotgunBulletCountIncrement * sign;
        //weaponTargetEffectTotal.criticalChanceIncrement += targetEffect.criticalChanceIncrement * sign;
        //weaponTargetEffectTotal += targetEffect * sign;        

        // 곱 옵션 - 합 연산
        weaponTargetEffectTotal.damageIncrement += targetEffect.damageIncrement * sign;
        weaponTargetEffectTotal.knockBackIncrement += targetEffect.knockBackIncrement * sign;
        weaponTargetEffectTotal.chargingAmountIncrement += targetEffect.chargingAmountIncrement * sign;
        weaponTargetEffectTotal.gettingSkillGaugeIncrement += targetEffect.gettingSkillGaugeIncrement * sign;
        weaponTargetEffectTotal.gettingStaminaIncrement += targetEffect.gettingStaminaIncrement * sign;
        weaponTargetEffectTotal.skillPowerIncrement += targetEffect.skillPowerIncrement * sign;
        weaponTargetEffectTotal.bulletScaleIncrement += targetEffect.bulletScaleIncrement * sign;
        weaponTargetEffectTotal.bulletRangeIncrement += targetEffect.bulletRangeIncrement * sign;
        weaponTargetEffectTotal.bulletSpeedIncrement += targetEffect.bulletSpeedIncrement * sign;

        // 곱 옵션 - 곱 연산
        if (1 == sign)
        {
            weaponTargetEffectTotal.cooldownReduction *= (1.0f - targetEffect.cooldownReduction);
            weaponTargetEffectTotal.chargeTimeReduction *= (1.0f - targetEffect.chargeTimeReduction);
            weaponTargetEffectTotal.accuracyIncrement *= (1.0f - targetEffect.accuracyIncrement);
            weaponTargetEffectTotal.shotgunsAccuracyIncrement = (1.0f - targetEffect.shotgunsAccuracyIncrement);
            //weaponTargetEffectTotal *= (1.0f - targetEffect);
        }
        else
        {
            weaponTargetEffectTotal.cooldownReduction /= (1.0f - targetEffect.cooldownReduction);
            weaponTargetEffectTotal.chargeTimeReduction /= (1.0f - targetEffect.chargeTimeReduction);
            weaponTargetEffectTotal.accuracyIncrement /= (1.0f - targetEffect.accuracyIncrement);
            weaponTargetEffectTotal.shotgunsAccuracyIncrement /= (1.0f - targetEffect.shotgunsAccuracyIncrement);
            //weaponTargetEffectTotal /= (1.0f - targetEffect);
        }

        // 미정
        weaponTargetEffectTotal.ammoCapacityIncrement += targetEffect.ammoCapacityIncrement * sign;

        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함. 
        if (targetEffect.canIncreasePierceCount)
            weaponTargetEffectTotal.canIncreasePierceCount = boolSign;
        if (targetEffect.becomesSpiderMine)
            weaponTargetEffectTotal.becomesSpiderMine = boolSign;
        if (targetEffect.bounceAble)
            weaponTargetEffectTotal.bounceAble = boolSign;
        if (targetEffect.shotgunBulletCanHoming)
            weaponTargetEffectTotal.shotgunBulletCanHoming = boolSign;
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
                RemoveTargetEffect(targetEffect, EffectApplyType.BUFF);
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
                RemoveTargetEffect(targetEffect, EffectApplyType.BUFF);
            }
            time += Time.fixedTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedTime);
        }
    }
}
