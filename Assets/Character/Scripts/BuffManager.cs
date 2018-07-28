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
    public WeaponTargetEffect[] WeaponTargetEffectTotal
    {
        get;
        private set;
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

            moveSpeedIncrement = 1f,
            rewardOfEndGameIncrement = 1f,

            discountRateOfVendingMachineItems = 1f,
            discountRateOfCafeteriaItems = 1f,
            discountRateAllItems = 1f,

            staminaMaxIncrement = 1f,
            canDrainHp = false
        };
    }

    /// <summary> 무기 대상 효과 종합 초기화 </summary>
    public void InitWeaponTargetEffectTotal()
    {
        int length = (int)WeaponAsset.WeaponType.END;
        WeaponTargetEffectTotal = new WeaponTargetEffect[length];

        WeaponTargetEffectTotal[0] = new WeaponTargetEffect()
        {
            bulletCountIncrement = 0,
            criticalChanceIncrement = 0,

            damageIncrement = 1f,
            knockBackIncrement = 1f,
            chargingDamageIncrement = 1f,
            gettingSkillGaugeIncrement = 1f,
            gettingStaminaIncrement = 1f,
            skillPowerIncrement = 1f,
            bulletScaleIncrement = 1f,
            bulletRangeIncrement = 1f,
            bulletSpeedIncrement = 1f,

            cooldownReduction = 1f,
            chargeTimeReduction = 1f,
            accuracyIncrement = 1f,

            ammoCapacityIncrement = 0,

            increasePierceCount = false,
            becomesSpiderMine = false,
            bounceAble = false,
            shotgunBulletCanHoming = false,
            meleeWeaponsCanBlockBullet = false,
            meleeWeaponsCanReflectBullet = false
        };

        for(int i = 1; i < length; i++)
        {
            WeaponTargetEffectTotal[i] = new WeaponTargetEffect()
            {
                cooldownReduction = 1f,
                chargeTimeReduction = 1f,
                accuracyIncrement = 1f
            };
        }
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
        // CharacterTargetEffectTotal.recoveryStamina += TargetEffect.recoveryStamina;

        // 합 연산
        CharacterTargetEffectTotal.armorIncrement += targetEffect.armorIncrement * sign;

        // 곱 옵션 - 합 연산
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
        CharacterTargetEffectTotal.staminaMaxIncrement += targetEffect.staminaMaxIncrement * sign;

        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함.
        if (targetEffect.canDrainHp)
            CharacterTargetEffectTotal.canDrainHp = boolSign;

        owner.ApplyItemEffect(characterTargetEffectTotal);
    }

    public void UpdateTargetEffectTotal(WeaponTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        int index = (int)targetEffect.weaponType;
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
        WeaponTargetEffectTotal[index].bulletCountIncrement += targetEffect.bulletCountIncrement * sign;
        WeaponTargetEffectTotal[index].criticalChanceIncrement += targetEffect.criticalChanceIncrement * sign;
        // WeaponTargetEffectTotal[index] += targetEffect * sign;        

        // 곱 옵션 - 합 연산
        WeaponTargetEffectTotal[index].damageIncrement += targetEffect.damageIncrement * sign;
        WeaponTargetEffectTotal[index].knockBackIncrement += targetEffect.knockBackIncrement * sign;
        WeaponTargetEffectTotal[index].chargingDamageIncrement += targetEffect.chargingDamageIncrement * sign;
        WeaponTargetEffectTotal[index].gettingSkillGaugeIncrement += targetEffect.gettingSkillGaugeIncrement * sign;
        WeaponTargetEffectTotal[index].gettingStaminaIncrement += targetEffect.gettingStaminaIncrement * sign;
        WeaponTargetEffectTotal[index].skillPowerIncrement += targetEffect.skillPowerIncrement * sign;
        WeaponTargetEffectTotal[index].bulletScaleIncrement += targetEffect.bulletScaleIncrement * sign;
        WeaponTargetEffectTotal[index].bulletRangeIncrement += targetEffect.bulletRangeIncrement * sign;
        WeaponTargetEffectTotal[index].bulletSpeedIncrement += targetEffect.bulletSpeedIncrement * sign;

        // 곱 옵션 - 곱 연산
        if (1 == sign)
        {
             WeaponTargetEffectTotal[index].cooldownReduction *= (1.0f - targetEffect.cooldownReduction);
             WeaponTargetEffectTotal[index].chargeTimeReduction *= (1.0f - targetEffect.chargeTimeReduction);
             WeaponTargetEffectTotal[index].accuracyIncrement *= (1.0f - targetEffect.accuracyIncrement);
            // WeaponTargetEffectTotal[index] *= (1.0f - targetEffect);
        }
        else
        {
             WeaponTargetEffectTotal[index].cooldownReduction /= (1.0f - targetEffect.cooldownReduction);
             WeaponTargetEffectTotal[index].chargeTimeReduction /= (1.0f - targetEffect.chargeTimeReduction);
             WeaponTargetEffectTotal[index].accuracyIncrement /= (1.0f - targetEffect.accuracyIncrement);
            // WeaponTargetEffectTotal[index] /= (1.0f - targetEffect);
        }

        // 미정
         WeaponTargetEffectTotal[index].ammoCapacityIncrement += targetEffect.ammoCapacityIncrement * sign;

        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함. 
        if (targetEffect.increasePierceCount)
             WeaponTargetEffectTotal[index].increasePierceCount = boolSign;
        if (targetEffect.becomesSpiderMine)
             WeaponTargetEffectTotal[index].becomesSpiderMine = boolSign;
        if (targetEffect.bounceAble)
             WeaponTargetEffectTotal[index].bounceAble = boolSign;
        if (targetEffect.shotgunBulletCanHoming)
             WeaponTargetEffectTotal[index].shotgunBulletCanHoming = boolSign;
        if (targetEffect.meleeWeaponsCanBlockBullet)
             WeaponTargetEffectTotal[index].meleeWeaponsCanBlockBullet = boolSign;
        if (targetEffect.meleeWeaponsCanReflectBullet)
             WeaponTargetEffectTotal[index].meleeWeaponsCanReflectBullet = boolSign;
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
