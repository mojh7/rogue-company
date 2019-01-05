using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 효과 적용 타입 : 1회성 소모품 ( 효과 적용 후 아이템 즉시 소멸 ) </summary>
[CreateAssetMenu(fileName = "ConsumableType", menuName = "ItemAsset/EffectApplyType/ConsumableType", order = 0)]
public class ConsumableType : EffectApplyType
{

    public override void UseItem()
    {
        if(caster)
        {
            for (int i = 0; i < itemUseEffect.Length; i++)
            {
                // Player 정보에 효과 적용 하고 아이템 바로 삭제
                caster.ApplyConsumableItem(itemUseEffect[i] as CharacterTargetEffect);
            }
            return;
        }
        for (int i = 0; i < itemUseEffect.Length; i++)
        {
            // Player 정보에 효과 적용 하고 아이템 바로 삭제
            PlayerManager.Instance.GetPlayer().ApplyConsumableItem(itemUseEffect[i] as CharacterTargetEffect);
        }
    }
}