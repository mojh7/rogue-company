using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Player </summary>
[CreateAssetMenu(fileName = "CharacterTargetEffect", menuName = "ItemAsset/ItemUseEffect/CharacterTargetEffect", order = 0)]
public class CharacterTargetEffect : ItemUseEffect
{
    // 합 옵션
    public float recoveryHp;
    public float recoveryHunger;
    public int armorIncrease;


    // 곱 옵션 - 합 연산
    public float criticalChanceIncrease; // 2. 치명타 확률 증가율
    public float moveSpeedIncrease;
    public float rewardOfEndGameIncrease;           // 게임 끝날 때 보상 증가율

    // 곱 옵션 - 곱 연산
    public float discountRateOfVendingMachineItems; // 자판기 아이템의 할인율
    public float discountRateOfCafeteriaItems;      // 카페테리아 아이템의 할인율
    public float discountRateAllItems;              // 모든 아이템의 할인율

    // 미정
    public float hungerMaxIncrease;
    // public float hpMaxIncrease;

    // on / off
    public bool canDrainHp; // 4. 흡혈 : 적 n명 처치당 체력 +0.5(반 칸) 회복
}