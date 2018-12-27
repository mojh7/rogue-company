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

    [Header("빙결 (= 아재 개그)")]
    [Range(0, 10)]
    public float freeze;
    [Range(0, 1)]
    public float freezeChance;

    [Header("기절 시간")]
    [Range(0, 10)]
    public float stun;
    [Range(0, 1)]
    public float stunChance;
    [Header("매혹 시간")]
    [Range(0, 10)]
    public float charm;
    [Range(0, 1)]
    public float charmChance;

    public Vector2 BulletPos { get; set; }
    public Vector2 BulletDir { get; set; }

    public StatusEffectInfo()
    {
        posionChance = 1f;
        burnChance = 1f;

        stunChance = 1f;
        charmChance = 1f;
    }

    public StatusEffectInfo(StatusEffectInfo info)
    {
        canPoison = info.canPoison;
        canBurn = info.canBurn;
        knockBack = info.knockBack;
        positionBasedKnockBack = info.positionBasedKnockBack;

        freeze = info.freeze;
        stun = info.stun;
        charm = info.charm;

        posionChance = info.posionChance;
        burnChance = info.burnChance;

        freezeChance = info.freezeChance;
        stunChance = info.stunChance;
        charmChance = info.charmChance;
    }
}