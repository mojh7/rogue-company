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
    public enum EffectApplyType { BUFF, PASSIVE, CONSUMABLEBUFF }

    // 소모형 버프 종류 늘어날 때 마다 따로 처리해야 되서, 0816 모
    public enum ConsumableBuffType { SHIELD }

    // 등록, 제거 여러 개 일 때 true, false 처리하기 위해서
    public enum CharacterBoolPropertyType { NONE, IS_NOT_CONSUME_STAMINA, IS_NOT_CONSUME_AMMO, END }
    public enum WeaponBoolPropertyType { NONE, END }

    #region variables
    private List<ItemUseEffect> passiveEffects;
    private int passiveEffectsLength;
    private List<ItemUseEffect> buffEffects;
    private int buffEffectsLength;

    // 소모형 버프 종류 늘어남에 따라 힙 따로 써야 됨. 현재는 캐릭터 쉴드 한 개만 있음 0816 모
    // TODO : 소모형 버프 안 넣을 수도 있어서 삭제 될 수도 있음.
    private Heap<ConsumableCharacterBuff> consumableBuffs;

    private Character owner;

    private CharacterTargetEffect characterTargetEffectTotal;
    private InGameTargetEffect inGameTargetEffectTotal;

    private int[] characterBoolPropertyCounts;
    private int[] weaponBoolPropertyCounts;
    #endregion

    #region get / set property
    public List<ItemUseEffect> PassiveEffects
    {
        get { return passiveEffects; }
    }
    //public int[] PassiveIds { get; private set; }
    public List<int> PassiveIds { get; private set; }
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
    public InGameTargetEffect InGameTargetEffectTotal
    {
        get { return inGameTargetEffectTotal; }
    }
    public WeaponTargetEffect[] WeaponTargetEffectTotal
    {
        get;
        private set;
    }
    #endregion

    #region initialization
    public void Init()
    {
        passiveEffects = new List<ItemUseEffect>();
        buffEffects = new List<ItemUseEffect>();
        PassiveIds = new List<int>();
        //PassiveIds = new int[ItemConstants.INVENTORY_MAX];
        consumableBuffs = new Heap<ConsumableCharacterBuff>(100);
        //characterTargetEffects = new List<CharacterTargetEffect>();
        //weaponTargetEffects = new List<WeaponTargetEffect>();
        characterBoolPropertyCounts = new int[(int)CharacterBoolPropertyType.END];
        weaponBoolPropertyCounts = new int[(int)WeaponBoolPropertyType.END];

        passiveEffectsLength = 0;
        buffEffectsLength = 0;
        InitCharacterTargetEffectTotal();
        InitInGameTargetEffectTotal();
        InitWeaponTargetEffectTotal();
    }

    /// <summary>
    /// owner 설정, owner 누구 껀지 꼭 구분 필요합니다. Init 이후의 함수 실행해주심 됨.
    /// </summary>
    public void SetOwner(Character owner)
    {
        this.owner = owner;
    }

    /// <summary> 캐릭터 대상 효과 종합 초기화 </summary>
    public void InitCharacterTargetEffectTotal()
    {
        characterTargetEffectTotal = new CharacterTargetEffect
        {
            // 합 옵션
            recoveryHp = 0,
            recoveryStamina = 0,
            armorIncrement = 0,
            gettingStaminaIncrement = 0,

            // 곱 옵션 - 합 연산
            moveSpeedIncrement = 1f,
            rewardOfEndGameIncrement = 1f,
            hpMaxRatio = 1,
            hpRatio = 1,
            charScale = 1f,

            // 곱 옵션 - 곱 연산
            discountRateOfVendingMachineItems = 1f,
            discountRateOfCafeteriaItems = 1f,
            discountRateAllItems = 1f,

            skillGage = 1,
            steminaGage = 1,

            staminaMaxIncrement = 0f,

            canDrainHp = false,
            increaseStaminaWhenkillingEnemies = false,
            isNotConsumeStamina = false,
            isNotConsumeAmmo = false
        };
    }

    /// <summary> 게임 대상 효과 종합 초기화 </summary>
    public void InitInGameTargetEffectTotal()
    {
        inGameTargetEffectTotal = new InGameTargetEffect
        {
            rateUpperPercent = new RateUpperPercent { Act = false, percent = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 } },
            bargain = 0,
            megaCoin = 0,
            buffAstrologer = false
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

            // 곱 옵션 - 합 연산
            damageIncrement = 1f,
            knockBackIncrement = 1f,
            chargingSpeedIncrement = 1f,
            chargingDamageIncrement = 1f,
            gettingSkillGaugeIncrement = 1f,
            //gettingStaminaIncrement = 1f,
            skillPowerIncrement = 1f,
            bulletScaleIncrement = 1f,
            bulletRangeIncrement = 1f,
            bulletSpeedIncrement = 1f,

            // 곱 옵션 - 곱 연산
            decreaseDamageAfterPierceReduction = 1f,
            cooldownReduction = 1f,
            accuracyIncrement = 1f,

            ammoCapacityIncrement = 0,

            increasePierceCount = false,
            becomesSpiderMine = false,
            bounceAble = false,
            shotgunBulletCanHoming = false,
            canHoming = false,
            meleeWeaponsCanBlockBullet = false,
            meleeWeaponsCanReflectBullet = false
        };

        // 곱 옵션 - 곱 연산
        for (int i = 1; i < length; i++)
        {
            WeaponTargetEffectTotal[i] = new WeaponTargetEffect()
            {
                decreaseDamageAfterPierceReduction = 1f,
                cooldownReduction = 1f,
                accuracyIncrement = 1f
            };
        }
    }
    #endregion

    #region function
    /// <summary> 효과 등록 </summary>
    /// <param name="itemUseEffect">효과 내용</param>
    /// <param name="effectiveTime">효과 적용 시간, default = -1, 0초과된 값 => 일정 시간 동안 효과 적용되는 버프 아이템</param>
    public void RegisterItemEffect(ItemUseEffect itemUseEffect, EffectApplyType effectApplyType, int passiveId, float effectiveTime = -1f)
    {
        Coroutine removeCoroutine = null;

        if (typeof(CharacterTargetEffect) == itemUseEffect.GetType())
        {
            CharacterTargetEffect targetEffect = itemUseEffect as CharacterTargetEffect;
            //characterTargetEffects.Add(targetEffect);
            //characterTargetEffectsLength += 1;
            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
            {
                if(EffectApplyType.CONSUMABLEBUFF == effectApplyType)
                {
                    removeCoroutine = StartCoroutine(RemoveBuffEffect(targetEffect, effectiveTime));
                }
                else if(EffectApplyType.BUFF == effectApplyType)
                    StartCoroutine(RemoveBuffEffect(targetEffect, effectiveTime));
            }
        }
        else if(typeof(InGameTargetEffect) == itemUseEffect.GetType())
        {
            InGameTargetEffect targetEffect = itemUseEffect as InGameTargetEffect;
            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
            {
                if (EffectApplyType.CONSUMABLEBUFF == effectApplyType)
                {
                    removeCoroutine = StartCoroutine(RemoveBuffEffect(targetEffect, effectiveTime));
                }
                else if (EffectApplyType.BUFF == effectApplyType)
                    StartCoroutine(RemoveBuffEffect(targetEffect, effectiveTime));
            }
        }
        else
        {
            WeaponTargetEffect targetEffect = itemUseEffect as WeaponTargetEffect;
            //weaponTargetEffects.Add(targetEffect);
            //weaponTargetEffectsLength += 1;
            UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REGISTER);
            if (effectiveTime > 0)
                StartCoroutine(RemoveBuffEffect(targetEffect, effectiveTime));
        }

        switch (effectApplyType)
        {
            case EffectApplyType.BUFF:
                buffEffects.Add(itemUseEffect);
                buffEffectsLength += 1;
                break;
            case EffectApplyType.PASSIVE:
                passiveEffects.Add(itemUseEffect);
                // Debug.Log("passive 등록 id : " + passiveId);
                PassiveIds.Add(passiveId);
                passiveEffectsLength += 1;
                PassiveItemSlot.Instance.UpdatePassiveItemUI();
                PassiveItemForDebug.Instance.UpdatePassiveItemUI();
                break;
            case EffectApplyType.CONSUMABLEBUFF:
                ConsumableCharacterBuff consumableCharacterBuff
                    = new ConsumableCharacterBuff(itemUseEffect as CharacterTargetEffect, effectiveTime, removeCoroutine);
                consumableBuffs.Add(consumableCharacterBuff);
                break;
            default:
                break;
        }
    }

    /// <summary> 캐릭터 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(CharacterTargetEffect targetEffect, EffectApplyType effectApplyType)
    {
        switch (effectApplyType)
        {
            case EffectApplyType.BUFF:
                buffEffects.Remove(targetEffect);
                buffEffectsLength -= 1;
                break;
            case EffectApplyType.PASSIVE:
                passiveEffects.Remove(targetEffect);
                passiveEffectsLength -= 1;
                break;
            case EffectApplyType.CONSUMABLEBUFF:
                
                break;
            default:
                break;
        }
        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }
    /// <summary> 게임 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(InGameTargetEffect targetEffect, EffectApplyType effectApplyType)
    {
        switch (effectApplyType)
        {
            case EffectApplyType.BUFF:
                buffEffects.Remove(targetEffect);
                buffEffectsLength -= 1;
                break;
            case EffectApplyType.PASSIVE:
                passiveEffects.Remove(targetEffect);
                passiveEffectsLength -= 1;
                break;
            case EffectApplyType.CONSUMABLEBUFF:
                break;
            default:
                break;
        }
        UpdateTargetEffectTotal(targetEffect, TargetEffectTotalUpdateType.REMOVE);
    }
    /// <summary> 무기 대상 버프 제거 </summary> 
    public void RemoveTargetEffect(WeaponTargetEffect targetEffect, EffectApplyType effectApplyType)
    {
        if (EffectApplyType.PASSIVE  == effectApplyType)
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


    // 만드는 중
    public void RemoveConsumableBuff(ConsumableBuffType type)
    {
        switch(type)
        {
            case ConsumableBuffType.SHIELD:
                consumableBuffs.RemoveFirst();
                break;
        }
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
        CharacterTargetEffectTotal.gettingStaminaIncrement += targetEffect.gettingStaminaIncrement * sign;

        // 곱 옵션 - 합 연산
        CharacterTargetEffectTotal.moveSpeedIncrement += targetEffect.moveSpeedIncrement * sign;
        CharacterTargetEffectTotal.rewardOfEndGameIncrement += targetEffect.rewardOfEndGameIncrement * sign;

        CharacterTargetEffectTotal.hpRatio += targetEffect.hpRatio * sign;
        CharacterTargetEffectTotal.hpMaxRatio += targetEffect.hpMaxRatio * sign;
        CharacterTargetEffectTotal.charScale += targetEffect.charScale * sign;

        // 곱 옵션 - 곱 연산
        if (1 == sign)
        {
            CharacterTargetEffectTotal.discountRateOfVendingMachineItems *= (1f - targetEffect.discountRateOfVendingMachineItems);
            CharacterTargetEffectTotal.discountRateOfCafeteriaItems *= (1f - targetEffect.discountRateOfCafeteriaItems);
            CharacterTargetEffectTotal.discountRateAllItems *= (1f - targetEffect.discountRateAllItems);
            CharacterTargetEffectTotal.skillGage *= targetEffect.skillGage;
            CharacterTargetEffectTotal.steminaGage *= targetEffect.steminaGage;
            CharacterTargetEffectTotal.staminaMaxIncrement *= targetEffect.staminaMaxIncrement;

            //CharacterTargetEffectTotal *= (1.0f - targetEffect);
        }
        else
        {
            CharacterTargetEffectTotal.discountRateOfVendingMachineItems /= (1f - targetEffect.discountRateOfVendingMachineItems);
            CharacterTargetEffectTotal.discountRateOfCafeteriaItems /= (1f - targetEffect.discountRateOfCafeteriaItems);
            CharacterTargetEffectTotal.discountRateAllItems /= (1f - targetEffect.discountRateAllItems);
            CharacterTargetEffectTotal.skillGage /= targetEffect.skillGage;
            CharacterTargetEffectTotal.steminaGage /= targetEffect.steminaGage;
            CharacterTargetEffectTotal.staminaMaxIncrement /= targetEffect.staminaMaxIncrement;

            //CharacterTargetEffectTotal /= (1.0f - targetEffect);
        }


        // bool형 on / off 종류, 해당 되는 항목들은 아이템 등록시 true, 제거시 false로 total 정보를 설정 함.
        if (targetEffect.canDrainHp)
            CharacterTargetEffectTotal.canDrainHp = boolSign;
        if (targetEffect.increaseStaminaWhenkillingEnemies)
            CharacterTargetEffectTotal.increaseStaminaWhenkillingEnemies = boolSign;
        if (targetEffect.isNotConsumeStamina)
            UpdateCharacterBoolProperty(CharacterBoolPropertyType.IS_NOT_CONSUME_STAMINA, sign);
        if (targetEffect.isNotConsumeAmmo)
            UpdateCharacterBoolProperty(CharacterBoolPropertyType.IS_NOT_CONSUME_AMMO, sign);
        if (targetEffect.isImmuneDamage != CharacterInfo.DamageImmune.NONE)
        {
            if(boolSign)
                CharacterTargetEffectTotal.isImmuneDamage = targetEffect.isImmuneDamage;
            else
                CharacterTargetEffectTotal.isImmuneDamage = CharacterInfo.DamageImmune.NONE;
        }
        if (targetEffect.isImmuneAbnormal != CharacterInfo.AbnormalImmune.NONE)
        {
            if (boolSign)
                CharacterTargetEffectTotal.isImmuneAbnormal = targetEffect.isImmuneAbnormal;
            else
                CharacterTargetEffectTotal.isImmuneAbnormal = CharacterInfo.AbnormalImmune.NONE;
        }
        owner.ApplyItemEffect();
        PassiveItemForDebug.Instance.UpdateEffectTotalNameText();
    }

    public void UpdateTargetEffectTotal(InGameTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        int sign;

        if (TargetEffectTotalUpdateType.REGISTER == updateType)
        {
            sign = 1;
        }
        // 제거
        else
        {
            sign = -1;
        }

        // 퍼센트 옵션 가격 할인
        inGameTargetEffectTotal.bargain += targetEffect.bargain * sign;

        if(sign == 1)
        {
            inGameTargetEffectTotal.rateUpperPercent.Act = targetEffect.rateUpperPercent.Act;
            for (int i = 0; i < 7; i++)
            {
                inGameTargetEffectTotal.rateUpperPercent.percent[i] += targetEffect.rateUpperPercent.percent[i];
            }
        }
        else
        {
            inGameTargetEffectTotal.rateUpperPercent.Act = targetEffect.rateUpperPercent.Act;
            for (int i = 0; i < 6; i++)
            {
                inGameTargetEffectTotal.rateUpperPercent.percent[i] -= targetEffect.rateUpperPercent.percent[i];
            }
        }
        owner.ApplyItemEffect();
        PassiveItemForDebug.Instance.UpdateEffectTotalNameText();
    }

    public void UpdateTargetEffectTotal(WeaponTargetEffect targetEffect, TargetEffectTotalUpdateType updateType)
    {
        for(int i = 0; i < targetEffect.weaponType.Length; i++)
        {
            int index = (int)targetEffect.weaponType[i];
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
            WeaponTargetEffectTotal[index].chargingSpeedIncrement += targetEffect.chargingSpeedIncrement * sign;
            WeaponTargetEffectTotal[index].chargingDamageIncrement += targetEffect.chargingDamageIncrement * sign;
            WeaponTargetEffectTotal[index].gettingSkillGaugeIncrement += targetEffect.gettingSkillGaugeIncrement * sign;
            //WeaponTargetEffectTotal[index].gettingStaminaIncrement += targetEffect.gettingStaminaIncrement * sign;
            WeaponTargetEffectTotal[index].skillPowerIncrement += targetEffect.skillPowerIncrement * sign;
            WeaponTargetEffectTotal[index].bulletScaleIncrement += targetEffect.bulletScaleIncrement * sign;
            WeaponTargetEffectTotal[index].bulletRangeIncrement += targetEffect.bulletRangeIncrement * sign;
            WeaponTargetEffectTotal[index].bulletSpeedIncrement += targetEffect.bulletSpeedIncrement * sign;
           

            // 곱 옵션 - 곱 연산
            if (1 == sign)
            {
                WeaponTargetEffectTotal[index].decreaseDamageAfterPierceReduction *= (1.0f - targetEffect.decreaseDamageAfterPierceReduction);
                WeaponTargetEffectTotal[index].cooldownReduction *= (1.0f - targetEffect.cooldownReduction);
                WeaponTargetEffectTotal[index].accuracyIncrement *= (1.0f - targetEffect.accuracyIncrement);
                // WeaponTargetEffectTotal[index] *= (1.0f - targetEffect);
            }
            else
            {
                WeaponTargetEffectTotal[index].decreaseDamageAfterPierceReduction /= (1.0f - targetEffect.decreaseDamageAfterPierceReduction);
                WeaponTargetEffectTotal[index].cooldownReduction /= (1.0f - targetEffect.cooldownReduction);
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
            if (targetEffect.canHoming)
                WeaponTargetEffectTotal[index].canHoming = boolSign;
            if (targetEffect.meleeWeaponsCanBlockBullet)
                WeaponTargetEffectTotal[index].meleeWeaponsCanBlockBullet = boolSign;
            if (targetEffect.meleeWeaponsCanReflectBullet)
                WeaponTargetEffectTotal[index].meleeWeaponsCanReflectBullet = boolSign;
        }

        PassiveItemForDebug.Instance.UpdateEffectTotalNameText();
    }

    private void UpdateCharacterBoolProperty(CharacterBoolPropertyType property, int sign)
    {
        characterBoolPropertyCounts[(int)property] += sign;
        switch (property)
        {
            case CharacterBoolPropertyType.IS_NOT_CONSUME_STAMINA:
                if (0 == characterBoolPropertyCounts[(int)property])
                    CharacterTargetEffectTotal.isNotConsumeStamina = false;
                else
                    CharacterTargetEffectTotal.isNotConsumeStamina = true;
                break;
            case CharacterBoolPropertyType.IS_NOT_CONSUME_AMMO:
                if (0 == characterBoolPropertyCounts[(int)property])
                    CharacterTargetEffectTotal.isNotConsumeAmmo = false;
                else
                    CharacterTargetEffectTotal.isNotConsumeAmmo = true;
                break;
            default:
                break;
        }
    }
    #endregion

    private IEnumerator RemoveBuffEffect(CharacterTargetEffect targetEffect, float effectiveTime)
    {
        float time = 0;
        yield return YieldInstructionCache.WaitForSeconds(effectiveTime);
        RemoveTargetEffect(targetEffect, EffectApplyType.BUFF);
    }

    private IEnumerator RemoveBuffEffect(InGameTargetEffect targetEffect, float effectiveTime)
    {
        float time = 0;
        yield return YieldInstructionCache.WaitForSeconds(effectiveTime);
        RemoveTargetEffect(targetEffect, EffectApplyType.BUFF);
    }

    private IEnumerator RemoveBuffEffect(WeaponTargetEffect targetEffect, float effectiveTime)
    {
        float time = 0;
        yield return YieldInstructionCache.WaitForSeconds(Time.fixedTime);
        RemoveTargetEffect(targetEffect, EffectApplyType.BUFF);
    }
}
