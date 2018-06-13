using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 예전에 쓴 것들 바꿔야됨.



/* Onwer 안에 속해서 각종 Buff를 관리
 * 
 * 패시브 아이템, 시간제 버프 아이템이 Onwer에게 생성되고 발생되면 여기에 등록되었다가
 * 삭제 될 때
 *  - 시간제 버프 아이템은 적용 시간 다 될 때
 *  - 게임이 완전이 끝나서 초기화 해야될 때 패시브 아이템 삭제
 *  - 무언가 상위 아이템으로 바뀌게 되서 기존의 아이템 삭제 해야 될 때
 * 여기에 등록 되었던 아이템 삭제
 * 
 * 
 * 버프 종류
 *  # 단순 능력치
 *   - 공격력 상승률, 치명타 상승률, 총알 사정거리 상승률 등등 float형이고 공격력 + 20% => +0.2f 식으로
 *     buffTotal에 더해져서 1.0f + 0.2f = 1.2f;
 *   - 버프 추가시에만 더하기 연산 하고 실제로 쓰는 weapon, bullet에서는 값만 읽어와서 기존 공격력에 값만 곱해줘서 사용하면 됨.
 *   
 *  # 추가 속성
 *   - ex : 레이저 무기 빙결 속성 추가 같은 특정 조건을 만족하는 무기에 대한 특정 속성 추가같은 경우
 *   - 추가후에 삭제시에 알맞는 거에 대한 삭제를 하려면 추가적인 조취를 해줘야 되서
 *     weapon, bullet 사용시에 weaponBuffList를 모두 순회하며 추가해야 될 property를 즉각적으로 읽어와서 추가하여 사용하면 되지 않을까 함.
 *     
 *  
 */


// 버프 류 생성 및 삭제할 때만 버프 내용 업데이트
// 사용되는 무기, 총알 쪽에서는 정보만 받아오기
public class BuffManager : MonoBehaviourSingleton<BuffManager>
{
    #region variables
    private List<WeaponBuff> weaponBuffList;
    private int weaponBuffListLength;
    private int numberBuffs;
    private WeaponBuff weaponBuffTotal;
    #endregion

    #region getter

    #endregion
    #region setter
    #endregion

    #region function

    public BuffManager()
    {
        weaponBuffList = new List<WeaponBuff>();
        weaponBuffTotal = new WeaponBuff();
        weaponBuffListLength = 0;
    }
    public void Init()
    {
        
    }
    public void UpdateAbility()
    {

    }

    // 버프 추가
    // 일단 무기 버프
    public void AddWeaponBuff(WeaponBuff buff)
    {
        weaponBuffList.Add(buff);
        weaponBuffListLength++;

        // 단순 능력치 추가
        weaponBuffTotal.info.cooldownIncreaseRate       += buff.info.cooldownIncreaseRate;
        weaponBuffTotal.info.damageIncreaseRate         += buff.info.damageIncreaseRate;
        weaponBuffTotal.info.criticalRateIncreaseRate   += buff.info.criticalRateIncreaseRate;
        weaponBuffTotal.info.knockBackIncreaseRate      += buff.info.knockBackIncreaseRate;
        weaponBuffTotal.info.ammoCapacityIncreaseRate   += buff.info.ammoCapacityIncreaseRate;

        weaponBuffTotal.info.bulletScaleIncreaseRate    += buff.info.bulletScaleIncreaseRate;
        weaponBuffTotal.info.bulletRangeIncreaseRate    += buff.info.bulletRangeIncreaseRate;
        weaponBuffTotal.info.bulletSpeedIncreaseRate    += buff.info.bulletSpeedIncreaseRate;

        weaponBuffTotal.info.chargeTimeIncreaseRate     += buff.info.chargeTimeIncreaseRate;
        weaponBuffTotal.info.chargeDamageIncreaseRate   += buff.info.chargeDamageIncreaseRate;

        // 임시
        weaponBuffTotal.info.bulletCountIncreaseCount += buff.info.bulletCountIncreaseCount;
        // 충돌 속성
        for(int i = 0; i < buff.info.addCollisionPropertiesLength; i++) 
        {
            weaponBuffTotal.info.addCollisionProperties.Add(buff.info.addCollisionProperties[i].Clone());
        }
        // update 속성
        for (int i = 0; i < buff.info.addUpdatePropertiesLength; i++)
        {
            weaponBuffTotal.info.addUpdateProperties.Add(buff.info.addUpdateProperties[i].Clone());
        }
        // 삭제 속성
        for (int i = 0; i < buff.info.addDeletePropertiesLength; i++)
        {
            weaponBuffTotal.info.addDeleteProperties.Add(buff.info.addDeleteProperties[i].Clone());
        }
    }

    // 버프 제거
    public void RemoveWeaponBuff(WeaponBuff buff)
    {
        for (int i = 0; i < weaponBuffListLength; i++)
        {
            if(weaponBuffList[i] == buff)
            {
                weaponBuffList.RemoveAt(i);
                weaponBuffListLength--;
                break;
            }
        }

        // 단순 능력치
        weaponBuffTotal.info.cooldownIncreaseRate -= buff.info.cooldownIncreaseRate;
        weaponBuffTotal.info.damageIncreaseRate -= buff.info.damageIncreaseRate;
        weaponBuffTotal.info.criticalRateIncreaseRate -= buff.info.criticalRateIncreaseRate;
        weaponBuffTotal.info.knockBackIncreaseRate -= buff.info.knockBackIncreaseRate;
        weaponBuffTotal.info.ammoCapacityIncreaseRate -= buff.info.ammoCapacityIncreaseRate;

        weaponBuffTotal.info.bulletScaleIncreaseRate -= buff.info.bulletScaleIncreaseRate;
        weaponBuffTotal.info.bulletRangeIncreaseRate -= buff.info.bulletRangeIncreaseRate;
        weaponBuffTotal.info.bulletSpeedIncreaseRate -= buff.info.bulletSpeedIncreaseRate;

        weaponBuffTotal.info.chargeTimeIncreaseRate -= buff.info.chargeTimeIncreaseRate;
        weaponBuffTotal.info.chargeDamageIncreaseRate -= buff.info.chargeDamageIncreaseRate;
    }
    #endregion
    
}
