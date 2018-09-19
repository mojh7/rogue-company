using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0806 모장현
// 각종 디버그 및 테스트를 위한 on / off 변수들

public enum WeaponModeForDebug { TEST, TEMP, TEMP2, TEST2, A1 }

/// <summary>
/// 디버그용 셋팅, 변수들
/// </summary>
public class DebugSetting : MonoBehaviourSingleton<DebugSetting>
{
    [Header("Enemy Data로 무기 설정")]
    public bool equipsEnemyDataWeapon = true;
    [Header("몬스터 테스트용으로 장착할 무기 id")]
    public int enemyEquipWeaponId;

    [Header("Player 무기 설정")]
    public WeaponModeForDebug weaponModeForDebug;
    [Header("Player 무기 모두 착용 ")]
    public bool EquipsPlayerAllWeapons = true;
    [Header("Player 착용 특정 index, 0이상 일 때 적용 ")]
    public int playerEquipWepaonId = -1;

    [Header("temp Player 무기 테스트 범위")]
    [Range(0, 45)]
    public int startWeaponIndex;
    [Range(1, 45)]
    public int endWeaponIndex;

    [Header("총구 pos 표시 for Debug")]
    public bool showsMuzzlePos;
}
