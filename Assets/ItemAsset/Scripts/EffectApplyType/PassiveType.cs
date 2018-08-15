using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 효과 적용 타입 : 패시브 ( 게임 끝나기 전까지 유지 ) </summary>
[CreateAssetMenu(fileName = "PassiveType", menuName = "ItemAsset/EffectApplyType/PassiveType", order = 2)]
public class PassiveType : EffectApplyType
{
    public override void UseItem()
    {
        // buffManager에 등록
        for (int i = 0; i < itemUseEffect.Length; i++)
        {
            PlayerBuffManager.Instance.BuffManager.RegisterItemEffect(itemUseEffect[i], BuffManager.EffectApplyType.PASSIVE);
        }
    }
}