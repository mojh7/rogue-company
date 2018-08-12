using UnityEngine;
using System.Collections;

/// <summary> 상태이상(CC기 포함) 효과 정보 </summary>
[System.Serializable]
public class StatusEffectInfo
{
    [Header("화상 및 독")]
    public bool canPoison;
    public bool canBurn;

    [Header("넉백 세기와 넉백 방식")]
    public float knockBack;
    public bool positionBasedKnockBack;

    [Header("이동 지연")]
    public bool canDelayState;

    [Header("잔소리")]
    public bool canNag;
    [Header("등산")]
    public bool canClimb;
    [Header("철야 근무")]
    public bool graveyardShift;
    [Header("빙결 (= 아재 개그)")]
    public bool canFreeze;
    [Header("청개구리")]
    public bool reactance;

    [Header("스턴 시간")]
    public float stun;
    [Header("매혹 시간")]
    public float charm;

    public Vector2 BulletPos { get; set; }
    public Vector2 BulletDir { get; set; }

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
    }
}