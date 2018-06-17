using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 아이템 적용 방법, 사용 방법에 따라서 구분됨.


public abstract class EffectApplyType : ScriptableObject
{
    [Tooltip("개발용 메모장")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    [Header("ConsumableType은 현재 PlayerTargetEffect만 적용")]
    [SerializeField]
    protected ItemUseEffect[] itemUseEffect;
    // protected int itemUseEffectsLength;

    protected bool removable;

    public abstract void UseItem();
}
