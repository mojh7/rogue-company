using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RateUpperPercent
{
    public bool Act;
    [Header("S,A,B,C,D,E")]
    public List<float> percent;
}
///<summary> 아이템 효과 상세 내용 및 대상 </summary>
[CreateAssetMenu(fileName = "InGameTargetEffect", menuName = "ItemAsset/ItemUseEffect/InGameTargetEffect", order = 2)]
public class InGameTargetEffect : ItemUseEffect
{
    [Header("버프 형")]

    [Header("복합 수치")]
    public RateUpperPercent rateUpperPercent;

    [Header("퍼센트 옵션")]
    public float bargain;

    [Header("소모 형")]

    [Header("단순 수치")]
    public int megaCoin;

    [Header("on/ off 속성")]
    public bool buffAstrologer;
}