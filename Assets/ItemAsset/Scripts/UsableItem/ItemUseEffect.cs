using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*
    # 플레이어 적용 효과들 구현

    ------- 만들 수 있을만한 것들 -------
    1. 회복 류
     - hp, 허기

    2. 일정시간 상승류
     - 운, 이동속도, 공격력

    ------- 회의, 고민 필요한 것들 -------
    3. 몬스터 공격력 덜 들어오는 방어력? 쉴드 개념
    
    4. 기타
     - 9층 구멍에 캐릭터가 닿았을 때 8층으로 떨어지지 않는다.
     - 9층 구멍에 캐릭터가 닿았을 때 8층으로 떨어져도 데미지를
       입지 않는다

    ----------------------

    # 무기관련 패시브 먼저 만들만한 것
    
    1. 단일 발사류(권총 류) 비관통 => 관통 on 아니면 관통 횟수 +1  
     - 관통 0회 => 관통 1회

    2. 공격속도 +10% ( = 쿨다운 -10%)

    3. 근접 무기 상대 총알 튕겨내기 추가
    
    4. 공격시 흡혈
    
    5. 몬스터 죽일 때 회복되는 허기량 상승
    
    6. 집탄률 상승
    
    7. 치명타 확률 상승
    
    8. 차징 무기 차지 시간 감소
    
    9. 공격 적중 시 특수 공격 게이지 상승량 증가


    무기 대상 효과 중 효과가 적용되는 타이밍(class 기준?) 일단 3곳

    1. Weapon 예시
     - 차징 시간 감소
     - 공격 속도 상승

    2. Pattern 예시
     - 집탄률 상승

    3. Bullet 예시
     - 치명타 확률 상승
     - 공격시 특수 공격
     - 비관통 총알 => 관통 총알
     - 공격시 흡혈
     - 근접 무기 상대 총알 튕겨내기 추가
     - 몬스터 죽일 때 회복되는 허기량 상승
 */



// 아이템 효과 상세 내용으로 주로 적용 대상에 따라서 구분 됨.

public abstract class ItemUseEffect
{

}


// 플레이어 대상 적용 효과
public class PlayerTargetEffect : ItemUseEffect
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

