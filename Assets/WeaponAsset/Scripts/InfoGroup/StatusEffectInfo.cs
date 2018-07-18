using UnityEngine;
using System.Collections;

/// <summary> 상태이상(CC기 포함) 효과 정보 </summary>
[System.Serializable]
public class StatusEffectInfo
{

    // 체력 손실
    public bool canPoison;
    //public float overlappingPoisonDamage;
    //public float overlappingPoisonEffectiveTime;

    public bool canBurn;

    // 이상 행동
    public float knockBack;
    public bool positionBasedKnockBack;

    // 잔소리
    public bool canNag;
    // 이동 지연
    public bool canDelayState;

    /*
    public float slow;
    public float slowEffectiveTime;

    public bool canStun;
    public float stunEffectiveTime;

    public bool canFreeze;
    public float freezeEffectiveTime;
    */

    // 효과들??
    // 슬로우 이동 방해
    // 빙결 이동 제한
    // 스턴 이동, 공격 제한
    // 공포 이동 제어(player 반대 방향), 공격 제한

    // 추가할 만한 것들
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
        
        /*
        slow = info.slow;
        slowEffectiveTime = info.slowEffectiveTime;

        canStun = info.canStun;
        stunEffectiveTime = info.stunEffectiveTime;

        canFreeze = info.canFreeze;
        freezeEffectiveTime = info.freezeEffectiveTime;
        */
    }
}