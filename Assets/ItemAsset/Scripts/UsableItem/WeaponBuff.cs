using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// ItemUseEffect 안의 내용으로 옮길 예정


/*
 * 패시브 아이템, 시간제 아이템 사용 시 무기 관련 버프를 얻게되는데
 * 얻은 버프가 onwer 하위에 있는 무기 모두에게 적용되므로
 * onwer안에 붙여서 사용
 * 
 * 현재 어떻게 확장할 지 몰라서 Buff 임시로 놓음
 * 
 * 디버프 기능도 쓸 수 있으면 쓸 수 있지 않을까
 * 
 * 
 * 
 * 특성 종류
 * 1. 무기 능력치 상승
 *  1-1. 
 *  
 * 2. 무기 특수 효과 부여
 *  2-1. 적을 죽였을 때 일정한 확률로 체력 회복
 * 
 * 
 */


public abstract class Buff
{
    public float effectiveTime;
}

/*
 * 고려 할 점.
 * 
 * 매번 공격할 때 마다(무기 버프 기준) 현재 활성화 되고 있는 모든 버프를
 * 반복문으로 돌면서 적용될 버프를 처리하기 보다는
 * 버프 효과가 적용되는 player, Weapon, pattern, bullet 에서 o(1)으로 처리하고 싶음.
 *  => 단순히 공격력, 이동속도 증가량 같은 단순 스텟 변화하는 버프는 추가 및 삭제가 편함.
 * 
 * 
 * 버프 획득, 삭제, 지속 시간, 효과를 얻는 형태, 
 * 습득, 소멸, 적용, 적용 시간
 * 
 * 이미지 필요 방식
 * 1. 2개 필요
 *  - 땅에 떨어졌을 때 그림
 *  - 아이콘 모양의 그림 ( 적용중인 버프 아이템, 소지중인 버프 아이템 확인용)
 *    
 * 2. 1개 필요 
 *  - 땅에 떨어졌을 때 그림 = 아이콘 모양의 그림
 * 
 * 
 * 
 * 1. 버프 적용 시간.
 *  - 소모형 아이템 : 얻음과 동시에 즉시 효과를 얻고 사라짐 (ex : HP 회목 아이템)
 *  - 시간제 버프형
 * 
 * 
 * 2. 얻는 위치
 *  - 땅에 떨어진 아이템 먹기(주로 음식 아이템 류)
 *  
 *  - 특정 위치에서 n개 중 1개 고르기 ( 소울나이트 특성 고르듯이)
 * 
 */


/// <summary>
/// int 증가 갯수force
/// float 증가 비율
/// 
/// 아직 비율 증가(float), 갯수 증가(int) 명확하지 않음.
/// 
/// 어떤 버프라도 특정 조건을 갖춘 무기, 총알에 대해서 적용되야 하는점.
/// es : 레이저 무기 공격력 상승, 근접 무기 넉백 률 증가 등등
/// 
/// 버프 종류에 따라서 업데이트, 적용 시점이 weapon, bulletPattern, bullet, bulletProperty 각자 다름.
/// </summary>
public class WeaponBuff : Buff
{
    #region variables
    public WeaponBuffInfo info;
    // 버프 매니저안 weaponBuffList에서의 index

    #endregion
    #region getter
    #endregion

    #region setter
    #endregion
    /*
     * 능력치가 상승할 때 종류
     * 1. %로 오르는게 여러개 일 때 합 연산 or 곱 연산
     * 
     * 무기1 공 20, 버프 1 공 + 5, 버프 2 공 + 2, 버프 3 공 + 10%, 버프 4 공 + 15%
     * (20 + 5 + 2) * (1.0 + 0.1 + 0.15) = 27 * 1.25 == 33.75
     * 
     * 정수형으로 할지 실수로 할 지에 따라서 어떻게 할지 고민좀 해야 될듯.
     * 
     * 공격력 상승
     * 
     */

    #region function

    // 미리 정보를 에디터로 저장하고 id 참조하는 방식으로 해야할 듯
    public void Init(int weaponBuffId)
    {

    }

    // buff total 용 초기화
    public void InitTotal()
    {
        info = new WeaponBuffInfo();
        
        // struct 자체의 초기화 부분으로 넘어 갈 수 예정.

        // 단순 능력치
        info.cooldownIncreaseRate += 1.0f;
        info.damageIncreaseRate += 1.0f;
        info.criticalRateIncreaseRate += 1.0f;
        info.knockBackIncreaseRate += 1.0f;
        info.ammoCapacityIncreaseRate += 1.0f;

        info.bulletScaleIncreaseRate += 1.0f;
        info.bulletRangeIncreaseRate += 1.0f;
        info.bulletSpeedIncreaseRate += 1.0f;

        info.chargeTimeIncreaseRate += 1.0f;
        info.chargeDamageIncreaseRate += 1.0f;

        info.bulletCountIncreaseCount += 0;

        info.addCollisionProperties = new List<CollisionProperty>();
        info.addUpdateProperties = new List<UpdateProperty>();
        info.addDeleteProperties = new List<DeleteProperty>();

        info.addCollisionPropertiesLength = 0;
        info.addUpdatePropertiesLength = 0;
        info.addDeletePropertiesLength = 0;
    }

    #endregion
}
