using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 효과 적용 타입 : 버프 ( 일정 시간 적용되다가 시간 지나면 소멸 됨 ) </summary>
[CreateAssetMenu(fileName = "BuffType", menuName = "ItemAsset/EffectApplyType/BuffType", order = 1)]
public class BuffType : EffectApplyType
{
    // 버프 지속 시간
    [SerializeField]
    private float effectiveTime;

    public override void UseItem()
    {
        // buffManager에 등록
        for (int i = 0; i < itemUseEffect.Length; i++)
        {
            PlayerBuffManager.Instance.BuffManager.RegisterItemEffect(itemUseEffect[i], BuffManager.EffectApplyType.BUFF, -1, effectiveTime);
        }
    }
}