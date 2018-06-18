using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Weapon </summary>
[CreateAssetMenu(fileName = "WeaponTargetEffect", menuName = "ItemAsset/ItemUseEffect/WeaponTargetEffect", order = 1)]
public class WeaponTargetEffect : ItemUseEffect
{
    // 단순히 능력치 증가 감소
    #region ability

    // 합 연산
    public float damageIncrease;           // 공격력 증가율
    public float criticalChanceIncrease;   // 치명타 확률 증가율, 플레이어에 적용되게 하면 모든 무기 적용이고 무기 종류에 따른 크리티컬 증감 효과 해야 될듯      
    public float knockBackIncrease;        // 넉백 증가율
    public float ammoCapacityIncrease;     // 탄창 Maximum 증가율, int형으로 갯수로 해야 될 수도
    public float chargeDamageIncrease;     // 차징 공격 데미지 증가율

    public float bulletScaleIncrease;      // 총알 크기 증가율
    public float bulletRangeIncrease;      // 총알 사정 거리 증가율
    public float bulletSpeedIncrease;      // 총알 속력 증가율

    // 곱 연산
    public float cooldownReduction;        // 무기 재사용 시간 감소율
    public float chargeTimeReduction;      // 차징 시간 감소율

    // 일정 수치 상승
    public int shotgunBulletCountIncrease;      // 샷건류(spread pattern) 총알 갯수 증가
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

    #endregion
}