using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Weapon </summary>
[CreateAssetMenu(fileName = "WeaponTargetEffect", menuName = "ItemAsset/ItemUseEffect/WeaponTargetEffect", order = 1)]
public class WeaponTargetEffect : ItemUseEffect
{
    // 단순히 능력치 증가 감소
    #region ability
    [Header("합 옵션")]
    public int shotgunBulletCountIncrease; // 샷건류(spread pattern) 총알 갯수 증가
    // public float criticalChanceIncrease;   // 모든 무기 크리티컬 확률 증가

    [Header("곱 옵션 - 합 연산")]
    public float damageIncrease;           // 3. 공격력 증가율
    public float knockBackIncrease;        // 넉백 증가율
    public float chargeDamageIncrease;     // 차징 공격 데미지 증가율
    public float bulletScaleIncrease;      // 총알 크기 증가율
    public float bulletRangeIncrease;      // 총알 사정 거리 증가율
    public float bulletSpeedIncrease;      // 총알 속력 증가율

    [Header("곱 옵션 - 곱 연산")]
    public float cooldownReduction;        // 9. 무기 재사용 시간 감소율
    public float chargeTimeReduction;      // 차징 시간 감소율
    public float accuracyIncrease;         // 집탄률, 탄 정확도 상승
    public float shotgunsAccuracyIncrease; // 샷건총 집탄률, 탄 정확도 상승
    
    // 미정
    public float ammoCapacityIncrease;     // 탄창 Maximum 증가율, int형으로 갯수로 해야 될 수도
    public float getTheHungerIncrease;     // 허기 획득량 상승
            
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

    public bool canIncreasePierceCount;     // 1. 비 관통 무기들 관통 횟수 +1 추가
    public bool becomesSpiderMine;          // 10. 함정 무기 스파이더 마인화
    public bool bounceAble;                 // 8. 총알이 벽에 1회 튕길 수 있음.
    public bool shotgunBulletCanHoming;     // 5. 샷건 총알 n초 후 유도 총알로 바뀜, n초 미정

    public bool blowWeaponsCanBlockBullet;       // 6. A분류 근거리 무기 총알 막기
    public bool swingWeaponsCanReflectBullet;    // 7. B분류 근거리 무기 총알 튕겨내기
    #endregion
}