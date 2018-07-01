using UnityEngine;
using System.Collections;

/// <summary> 상태이상(CC기 포함) 효과 정보 </summary>
[System.Serializable]
public class StatusEffectInfo
{
    [HideInInspector]
    public Vector2 bulletPos;
    [HideInInspector]
    public Vector2 bulletDir;

    // 지속 데미지류
    public bool canPoison;
    //public float overlappingPoisonDamage;
    //public float overlappingPoisonEffectiveTime;

    public bool canBurn;

    // 이동, 공격 등 상태 방해 제한류
    public float knockBack;
    public bool positionBasedKnockBack;

    public float slow;
    public float slowEffectiveTime;

    public bool canStun;
    public float stunEffectiveTime;

    public bool canFreeze;
    public float freezeEffectiveTime;

    // 효과들??
    // 슬로우 이동 방해
    // 빙결 이동 제한
    // 스턴 이동, 공격 제한
    // 공포 이동 제어(player 반대 방향), 공격 제한

    // 추가할 만한 것들
    // 공포, 매혹
    // 넉백도 여기로?

    public StatusEffectInfo(StatusEffectInfo info)
    {
        canPoison = info.canPoison;
        canBurn = info.canBurn;
        knockBack = info.knockBack;
        positionBasedKnockBack = info.positionBasedKnockBack;
        slow = info.slow;
        slowEffectiveTime = info.slowEffectiveTime;

        canStun = info.canStun;
        stunEffectiveTime = info.stunEffectiveTime;

        canFreeze = info.canFreeze;
        freezeEffectiveTime = info.freezeEffectiveTime;
    }
}