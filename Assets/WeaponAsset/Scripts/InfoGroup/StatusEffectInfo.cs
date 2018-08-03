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

    [Header("잔소리")]
    public bool canNag;

    [Header("이동 지연")]
    public bool canDelayState;

    [Header("슬로우 강도, 시간")]
    public float slow;
    public float slowEffectiveTime;

    [Header("스턴 시간")]
    public float stun;
    [Header("빙결 (= 아재 개그) 시간")]
    public float freeze;
    [Header("속박 시간")]
    public float root;
    [Header("공포 시간")]
    public float fear;
    [Header("매혹 시간")]
    public float charm;
    //[Header("")]
    // 공포, 매혹
    // 넉백도 여기로?

    public Vector2 BulletPos { get; set; }
    public Vector2 BulletDir { get; set; }

    public StatusEffectInfo(StatusEffectInfo info)
    {
        canPoison = info.canPoison;
        canBurn = info.canBurn;
        knockBack = info.knockBack;
        positionBasedKnockBack = info.positionBasedKnockBack;
        canNag = info.canNag;
        canDelayState = info.canDelayState;
        slow = info.slow;
        slowEffectiveTime = info.slowEffectiveTime;
        stun = info.stun;
        freeze = info.freeze;
        root = info.root;
        fear = info.fear;
        charm = info.charm;
    }
}