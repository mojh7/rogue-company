using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Player </summary>
[CreateAssetMenu(fileName = "PlayerTargetEffect", menuName = "ItemAsset/ItemUseEffect/PlayerTargetEffect", order = 0)]
public class PlayerTargetEffect : ItemUseEffect
{
    // 일정 량 회복
    public float recoveryHp;
    public float recoveryHunger;
    // public float hpMaxIncrease;

    // 일정 수치 증가
    public int armorIncrease;

    // ???
    public float hungerMaxIncrease;

    // 합 공식
    public float moveSpeedIncrease;
    public float criticalChanceIncrease; // 2. 치명타 확률 증가율

    // 기타 옵션
    public float discountRateOfVendingMachineItems; // 자판기 아이템의 할인율
    public float discountRateOfCafeteriaItems;      // 카페테리아 아이템의 할인율
    public float discountRateAllItems;              // 모든 아이템의 할인율
    public float rewardOfEndGameIncrease;           // 게임 끝날 때 보상 증가율
    // on / off
    public bool canDrainHp; // 4. 흡혈 : 적 n명 처치당 체력 +0.5(반 칸) 회복

}