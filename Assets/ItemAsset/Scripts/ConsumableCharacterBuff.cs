using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableCharacterBuff : IHeapItem<ConsumableCharacterBuff>
{

    private CharacterTargetEffect characterTargetEffect;
    private float effectiveTime;
    private Coroutine removeBuffCoroutine;

    public ConsumableCharacterBuff(CharacterTargetEffect characterTargetEffect, float effectiveTime, Coroutine removeBuffCoroutine)
    {
        this.characterTargetEffect = characterTargetEffect;
        this.effectiveTime = effectiveTime;
        this.removeBuffCoroutine = removeBuffCoroutine;
    }
    //public CharacterTargetEffect CharacterTargetEffect
    //{
    //    get; set;
    //}
    int heapIndex;

    public CharacterTargetEffect CharacterTargetEffect
    {
        get
        {
            return characterTargetEffect;
        }
    }

    public float EffectiveTime
    {
        get
        {
            return effectiveTime;
        }
    }

    public Coroutine RemoveBuffCoroutine
    {
        get
        {
            return removeBuffCoroutine;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }
    public int CompareTo(ConsumableCharacterBuff consumableBuff)
    {
        if (effectiveTime > consumableBuff.EffectiveTime)
            return 1;
        else if (effectiveTime == consumableBuff.EffectiveTime)
            return 0;
        else
            return -1;
    }
}