using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> 효과 적용 타입 : 소모형 버프 ( 일정 조건 충족시 적용 시간이 남았음에도 효과 사라짐.) </summary>
[CreateAssetMenu(fileName = "ConsumableBuffType", menuName = "ItemAsset/EffectApplyType/ConsumableBuffType", order = 3)]
public class ConsumableBuffType : EffectApplyType
{
    // 버프 지속 시간
    [SerializeField]
    private float effectiveTime;

    public override void UseItem()
    {
        // buffManager에 등록
        for (int i = 0; i < itemUseEffect.Length; i++)
        {
            PlayerBuffManager.Instance.BuffManager.RegisterItemEffect(itemUseEffect[i], BuffManager.EffectApplyType.CONSUMABLEBUFF, effectiveTime);
        }
    }
}