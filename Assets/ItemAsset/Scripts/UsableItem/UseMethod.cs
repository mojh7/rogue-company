using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// 아이템 적용 방법, 사용 방법에 따라서 구분됨.

public abstract class UseMethod
{
    protected List<ItemUseEffect> itemUseEffects;
    protected int itemUseEffectsLength;

    protected bool removable;

    public virtual void UseItem()
    {
    }
}

// 1회성 소모품 효과 ( 즉시 소멸 )
public class ConsumeableUse : UseMethod
{
    public override void UseItem()
    {
        base.UseItem();
        // Player singleton instance Player Info에 효과 적용 하고 아이템 바로 삭제
    }
}

// 버프 효과 ( 일정 시간 적용되다가 시간 지나면 소멸 됨)
public class BuffUse : UseMethod
{
    // 버프 지속 시간
    private float effectiveTime;

    public override void UseItem()
    {
        base.UseItem();
        // Player singleton instance get BuffManager register Buff Item, 일정 시간 지난 후 버프매니저에서 버프 아이템 효과 삭제
    }
}

// 패시브 효과 ( 게임 끝나기 전까지 유지)
public class PassiveUse : UseMethod
{
    public override void UseItem()
    {
        base.UseItem();
        // Player singleton instance get BuffManager register Passive Item, 게임 끝날 때 까지 적용
    }
}
