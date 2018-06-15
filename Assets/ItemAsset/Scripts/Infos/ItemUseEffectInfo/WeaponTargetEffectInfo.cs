using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTargetEffectInfo : ItemUseEffect
{

    // 단순히 능력치 증가 감소
    #region ability
    public float cooldownReduction;        // 쿨타임 감소율
    public float damageIncrease;           // 공격력 증가율
    public float criticalChanceIncrease;   // 치명타 확률 증가율, 플레이어에 적용되게 하면 모든 무기 적용이고 무기 종류에 따른 크리티컬 증감 효과 해야 될듯      
    public float knockBackIncrease;        // 넉백 증가율
    public float ammoCapacityIncrease;     // 탄창 Maximum 증가율, int형으로 갯수로 해야 될 수도

    public float bulletScaleIncrease;      // 총알 크기 증가율
    public float bulletRangeIncrease;      // 총알 사정 거리 증가율
    public float bulletSpeedIncrease;      // 총알 속력 증가율

    public float chargeTimeIncrease;       // 차징 시간 증가율
    public float chargeDamageIncrease;     // 차징 공격 데미지 증가율

    // 임시
    public int shotgunBulletCountIncrease;      // 샷건류 총알 갯수 증가
    #endregion

    /* 총알 속성 추가 해야 되는 것들
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