using UnityEngine;
using System.Collections;

public class StatusEffectInfo
{
    public float nonOverlappingPoisonDamage;
    public float nonOverlappingPoisonEffectiveTime;
    public float overlappingPoisonDamage;
    public float overlappingPoisonEffectiveTime;

    public float nonOverlappingBurnDamage;
    public float nonOverlappingBurnEffectiveTime;
    public float overlappingBurnDamage;
    public float overlappingBurnEffectiveTime;

    public float slow;
    public float slowEffectiveTime;

    public bool canStun;
    public float stunEffectiveTime;

    public bool canFreeze;
    public float freezeEffectiveTime;

}