using UnityEngine;
using System.Collections;

/// <summary> 상태이상(CC기 포함) 효과 정보 </summary>
[System.Serializable]
public class StatusEffectInfo
{
    public float nonOverlappingPoisonDamage;
    public float nonOverlappingPoisonEffectiveTime;
    public float overlappingPoisonDamage;
    public float overlappingPoisonEffectiveTime;

    public bool canBurn;

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
}