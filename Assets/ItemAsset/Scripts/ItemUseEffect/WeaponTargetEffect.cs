using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 내용 추가 할 때 마다 PassiveItemForDebug.cs 에 내용 추가하기.

///<summary> 아이템 효과 상세 내용 및 대상 : Weapon </summary>
[CreateAssetMenu(fileName = "WeaponTargetEffect", menuName = "ItemAsset/ItemUseEffect/WeaponTargetEffect", order = 1)]
public class WeaponTargetEffect : ItemUseEffect
{
    [Header("적용할 무기 범위, Default가 NULL이고 NULL로 설정시 => 모든 무기 적용")]
    public WeaponAsset.WeaponType weaponType;

    // 단순히 능력치 증가 감소
    #region ability
    [Header("합 옵션")]
    public int bulletCountIncrement;        // 총알 갯수 증가 - 주로 샷건류(spread pattern) 에서 사용
    public float criticalChanceIncrement;   // 무기 크리티컬 확률 증가

    [Header("곱 옵션 - 합 연산")]
    public float damageIncrement;           // 공격력 증가율
    public float knockBackIncrement;        // 넉백 증가율
    public float chargingAmountIncrement;   // 차징량 증가율
    public float gettingSkillGaugeIncrement;// 스킬 게이지 획득량 증가
    public float gettingStaminaIncrement;   // 스테미너 획득량 상승
    public float skillPowerIncrement;       // 스킬 파워(시간, 세기 등등) 증가율
    public float bulletScaleIncrement;      // 총알 크기 증가율
    public float bulletRangeIncrement;      // 총알 사정 거리 증가율
    public float bulletSpeedIncrement;      // 총알 속력 증가율

    [Header("곱 옵션 - 곱 연산")]
    public float cooldownReduction;         // 무기 재사용 시간 감소율
    public float chargeTimeReduction;       // 차징 시간 감소율
    public float accuracyIncrement;         // 집탄률, 탄 정확도 상승
    
    // 미정
    public float ammoCapacityIncrement;     // 탄창 Maximum 증가율, int형으로 갯수로 해야 될 수도


    #endregion
    /* 총알 속성 추가 해야 되는 것들, on / off 류
     * 
     * 상태 이상 공격 
     * 반사되는 총알
     * 공격시 흡혈
     * 관통 총알
     * 비추적 총알 => 유도 총알
     * 등등
     */
    #region addProperties
    [Header("on / off 속성")]
    public bool increasePierceCount;             // 무기 관통 횟수 1회 추가, 추후 합 옵션으로 이동 할 수도 있음.
    public bool becomesSpiderMine;               // 함정 무기 스파이더 마인화
    public bool bounceAble;                      // 총알이 벽에 1회 튕길 수 있음.
    public bool shotgunBulletCanHoming;          // 샷건 총알 n초 후 유도 총알로 바뀜, n초 미정

    public bool meleeWeaponsCanBlockBullet;      // 근거리 무기 총알 막기
    public bool meleeWeaponsCanReflectBullet;    // 근거리 무기 총알 튕겨내기
    #endregion
}