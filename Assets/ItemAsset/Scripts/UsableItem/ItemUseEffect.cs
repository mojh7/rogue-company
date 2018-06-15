using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 아이템 효과 상세 내용으로 주로 적용 대상에 따라서 구분 됨.

public abstract class ItemUseEffect
{

}


// 플레이어 대상 적용 효과
public class PlayerTargetEffect : ItemUseEffect
{
    public float recoveryHp;
    public float moveSpeedIncreaseRate;
    public float recoveryHunger;
    public float criticalChanceIncreaseRate;
}


// 무기 대상 적용 효과
public class WeaponTargetEffect : ItemUseEffect
{

    // increaseRate 변수명 길어서 짧은거로 하던지 뺄 수도 있음

    // 단순히 능력치 증가 감소
    #region ability
    public float cooldownIncreaseRate;         // 쿨타임 증가율
    public float damageIncreaseRate;           // 공격력 증가율
    public float criticalChanceIncreaseRate;   // 치명타 확률 증가율, 플레이어에 적용되게 하면 모든 무기 적용이고 무기 종류에 따른 크리티컬 증감 효과 해야 될듯      
    public float knockBackIncreaseRate;        // 넉백 증가율
    public float ammoCapacityIncreaseRate;     // 탄창 Maximum 증가율, int형으로 갯수로 해야 될 수도

    public float bulletScaleIncreaseRate;      // 총알 크기 증가율
    public float bulletRangeIncreaseRate;      // 총알 사정 거리 증가율
    public float bulletSpeedIncreaseRate;      // 총알 속력 증가율

    public float chargeTimeIncreaseRate;       // 차징 시간 증가율
    public float chargeDamageIncreaseRate;     // 차징 공격 데미지 증가율

    // 임시
    public int shotgunBulletCountIncreaseCount;      // 샷건류 총알 갯수 증가
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

