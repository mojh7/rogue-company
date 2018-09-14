using UnityEngine;
using System.Collections;

/// <summary> 상태이상(CC기 포함) 효과 정보 </summary>
[System.Serializable]
public class StatusEffectInfo
{
    [Header("화상 및 독")]
    public bool canPoison;
    [Range(0, 1)]
    public float posionChance;
    public bool canBurn;
    [Range(0, 1)]
    public float burnChance;

    [Header("넉백 세기와 넉백 방식")]
    public float knockBack;
    public bool positionBasedKnockBack;

    [Header("이동 지연")]
    public bool canDelayState;
    [Range(0, 1)]
    public float delayStateChance;

    [Header("잔소리")]
    public bool canNag;
    [Range(0, 1)]
    public float nagChance;
    [Header("등산")]
    public bool canClimb;
    [Range(0, 1)]
    public float climbChance;
    [Header("철야 근무")]
    public bool graveyardShift;
    [Range(0, 1)]
    public float graveyardShiftChance;
    [Header("빙결 (= 아재 개그)")]
    public bool canFreeze;
    [Range(0, 1)]
    public float freezeChance;
    [Header("청개구리")]
    public bool reactance;
    [Range(0, 1)]
    public float reactanceChance;

    [Header("스턴 시간")]
    public float stun;
    [Range(0, 1)]
    public float stunChance;
    [Header("매혹 시간")]
    public float charm;
    [Range(0, 1)]
    public float charmChance;

    public Vector2 BulletPos { get; set; }
    public Vector2 BulletDir { get; set; }

    public StatusEffectInfo()
    {
        posionChance = 1f;
        burnChance = 1f;
        delayStateChance = 1f;
        nagChance = 1f;

        climbChance = 1f;
        graveyardShiftChance = 1f;
        freezeChance = 1f;
        reactanceChance = 1f;
    }

    public StatusEffectInfo(StatusEffectInfo info)
    {
        canPoison = info.canPoison;
        canBurn = info.canBurn;
        knockBack = info.knockBack;
        positionBasedKnockBack = info.positionBasedKnockBack;
        canDelayState = info.canDelayState;

        canNag = info.canNag;
        canClimb = info.canClimb;
        graveyardShift = info.graveyardShift;
        canFreeze = info.canFreeze;
        reactance = info.reactance;

        stun = info.stun;
        charm = info.charm;

        posionChance = info.posionChance;
        burnChance = info.burnChance;
        delayStateChance = info.delayStateChance;
        nagChance = info.nagChance;
        climbChance = info.climbChance;
        graveyardShiftChance = info.graveyardShiftChance;
        freezeChance = info.freezeChance;
        reactanceChance = info.reactanceChance;

    }
}