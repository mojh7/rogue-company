using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



// 아이템 효과 상세 내용으로 주로 적용 대상에 따라서 구분 됨.

public abstract class ItemUseEffect
{

}


// 플레이어 대상 적용 효과
public class PlayerEffect : ItemUseEffect
{
    private float recoveryHp;
    private float moveSpeedIncreaseRate;
    private float recoveryHunger;
    private float ciriticalChanceIncreaseRate;


    /*
     * 맵에 있는 구멍 같은 것 떨어졌을 때 막는 다던가 그런 특수 한 케이스는 따로 클래스를 하나 더 만들던지
     * player 안의 bool 값들 만들어 놓고 true / false 만 해줘도 될 듯.
     * 
     */ 
}


// weaponBuff 내용에서 옮겨 와야됨.
// 무기 대상 적용 효과
public class WeaponEffect : ItemUseEffect
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

