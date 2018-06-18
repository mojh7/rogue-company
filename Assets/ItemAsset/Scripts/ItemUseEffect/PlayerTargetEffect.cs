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
    public float criticalChanceIncrease;
}