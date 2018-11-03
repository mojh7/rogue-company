using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableWithoutPlayer", menuName = "ItemAsset/EffectApplyType/ConsumableWithoutPlayer", order = 3)]
public class ItemWithoutPlayerType : EffectApplyType
{

    public override void UseItem()
    {
        // buffManager에 등록
        for (int i = 0; i < itemUseEffect.Length; i++)
        {
            Use(itemUseEffect[i] as InGameTargetEffect);
        }
    }

    private void Use(InGameTargetEffect inGameTargetEffect)
    {
        //패시브
        if (inGameTargetEffect.rateUpperPercent.Act ||
            inGameTargetEffect.bargain > 0                  
            )
        {
            PlayerBuffManager.Instance.BuffManager.RegisterItemEffect(inGameTargetEffect, BuffManager.EffectApplyType.PASSIVE, itemId);
        }

        if (inGameTargetEffect.megaCoin > 0)
        {
            for(int i=0;i<inGameTargetEffect.megaCoin;i++)
            {
                ItemManager.Instance.CreateItem(ItemManager.Instance.DropCoin(), position, new Vector2(Random.Range(-1f, 1f), Random.Range(3, 8)));
            }
        }
        if(inGameTargetEffect.buffAstrologer)
        {
            ParticleManager.Instance.PlayParticle("Twinkle", position);
            PlayerBuffManager.Instance.ApplyAstrologerBuff();
        }
    }
}