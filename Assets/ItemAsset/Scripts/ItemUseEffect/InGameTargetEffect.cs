using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 아이템 효과 상세 내용 및 대상 : Player </summary>
[CreateAssetMenu(fileName = "InGameTargetEffect", menuName = "ItemAsset/ItemUseEffect/InGameTargetEffect", order = 2)]
public class InGameTargetEffect : ItemUseEffect
{
    [Header("퍼센트 옵션")]
    public float Bargain;

    [Header("on/ off 속성")]
    public bool buffAstrologer;

}