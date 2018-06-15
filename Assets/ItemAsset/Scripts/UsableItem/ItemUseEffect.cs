using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 아이템 효과 상세 내용으로 주로 적용 대상에 따라서 구분 됨.

public abstract class ItemUseEffect
{

}


// 플레이어 대상 적용 효과
public class PlayerTargetEffect : ItemUseEffect
{
    private PlayerTargetEffectInfo info;

    public PlayerTargetEffectInfo Info
    {
        get { return info; }
    }
}


// 무기 대상 적용 효과
public class WeaponTargetEffect : ItemUseEffect
{
    private WeaponTargetEffectInfo info;

    public WeaponTargetEffectInfo Info
    {
        get { return info; }
    }
}

