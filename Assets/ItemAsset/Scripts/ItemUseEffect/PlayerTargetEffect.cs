using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Player </summary>
[CreateAssetMenu(fileName = "PlayerTargetEffect", menuName = "ItemAsset/ItemUseEffect/PlayerTargetEffect", order = 0)]
public class PlayerTargetEffect : ItemUseEffect
{
    public float recoveryHp;
    public float recoveryHunger;
    // public float hpMaxIncrease;
    public float hungerMaxIncrease;
    public float moveSpeedIncrease;
    public float criticalChanceIncrease;

    public int armorIncrease;
}