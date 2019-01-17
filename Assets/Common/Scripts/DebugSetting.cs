using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0806 모장현
// 각종 디버그 및 테스트를 위한 on / off 변수들

public enum WeaponModeForDebug { TEST, MAIN, SHAPE_SAMPLE, TEST_BOSS }
public enum WeaponEquipType { PLAYER_INFO, DEBUG, ALL}

/// <summary>
/// 디버그용 셋팅, 변수들
/// </summary>
public class DebugSetting : MonoBehaviourSingleton<DebugSetting>
{
    [Header("튜토리얼 씬 true, 아닐 때 false값으로 설정 해주세요")]
    public bool isTutorialScene;

    [Header("Player 무기 list 종류 설정")]
    public WeaponModeForDebug weaponModeForDebug;
    [Header("Player 정보, Debug용, 모든 무기 착용")]
    public WeaponEquipType weaponEquipType;
    [Header("weaponEquipType Debug 일 때 적용")]
    public int[] startingWeaponInfos;
    //[Header("Player 착용 특정 index, 0이상 일 때 적용 ")]
    //public int playerEquipWepaonId = -1;

    [Header("총구 pos 표시 for Debug")]
    public bool showsMuzzlePos;

    [Header("Enemy Data로 무기 설정")]
    public bool equipsEnemyDataWeapon = true;
    [Header("equipsEnemyDataWeapon false 일 때 몬스터 테스트용으로 장착할 무기 id 1개")]
    public int enemyEquipWeaponId;
}
