using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Player </summary>
[CreateAssetMenu(fileName = "CharacterTargetEffect", menuName = "ItemAsset/ItemUseEffect/CharacterTargetEffect", order = 0)]
public class CharacterTargetEffect : ItemUseEffect
{
    [Header("합 옵션")]
    public float    recoveryHp;
    public int      recoveryStamina;
    public int      armorIncrement; // 아직 안씀
    public int      gettingStaminaIncrement;

    [Header("곱 옵션 - 합 연산")]
    public float moveSpeedIncrement;
    public float hpMaxRatio;
    public float hpRatio;
    public float charScale;
    public float skillGage;
    public float staminaGage;

    [Header("곱 옵션 - 곱 연산")]
    // 미정
    public float staminaMaxIncrement;


    [Header("on/ off 속성")]
    public bool canDrainHp; // 4. 흡혈 : 적 7명 처치당 체력 +1 회복
    public bool increaseStaminaWhenkillingEnemies;    // 15. 적 죽일 때 얻는 스테미너량 증가
    public bool isNotConsumeStamina;
    public bool isNotConsumeAmmo;
    public CharacterInfo.DamageImmune isImmuneDamage;
    public CharacterInfo.AbnormalImmune isImmuneAbnormal;

}