using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 캐릭터 여러 개 만들 때 방법 및 관리 고민
 * 
 * 1. Player 클래스 상속 받은 자식 클래스 여러개 사용, 캐릭터(음악, 운동, 군인 등등)하나 하나씩 자식 클래스로 두어서 관리
 * 
 * 2. Player Class 한 개로만 쓰고 따로 Player Info 클래스로 만들어서 scriptable class로 정보 관리
 * 
 * 3. 1 + 2번 혼합 방식
 * 
 * 아마 2번으로 할 듯
 */

 // armor 쉴드 개념 있는 아이템들 있어서 아예 player 데미지 입는 공식
 // 데미지 - 방어력으로 하고 저런 아이템 먹으면 방어력 수치 잠깐 올렸다가 내릴려고 함.

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{

    #region variables

    [SerializeField]
    private Player.PlayerType playerType;
    [SerializeField]
    private Sprite playerSprite;

    [SerializeField]
    private float hp;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float hunger;
    [SerializeField]
    private float hungerMax;
    [SerializeField]
    private float armor;
    [SerializeField]
    private float criticalChance;

    /*
     * 패시브
     * passiveItem;
     * 
     * 액티브
     * activeSkill
     */

    [SerializeField]
    private WeaponInfo[] startingWeaponInfos;

    // 

    #endregion

    #region get/set Property

    public float Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    public float Hunger
    {
        get { return hunger; }
        set { hunger = value; }
    }
    public float HungerMax
    {
        get { return hungerMax; }
        set { hungerMax = value; }
    }

    public float Armor
    {
        get { return armor; }
        set { armor = value; }
    }
    public float CriticalChance
    {
        get { return criticalChance; }
        set { criticalChance = value; }
    }
    public WeaponInfo[] StartingWeaponInfos
    {
        get { return startingWeaponInfos; }
    }

    #endregion

    public PlayerData Clone()
    {
        PlayerData clonedInfo = CreateInstance<PlayerData>();

        clonedInfo.playerType = playerType;
        clonedInfo.playerSprite = playerSprite;
        clonedInfo.hp = hp;
        clonedInfo.moveSpeed = moveSpeed;
        clonedInfo.hunger = hunger;
        clonedInfo.hungerMax = hungerMax;
        clonedInfo.armor = armor;
        clonedInfo.criticalChance = criticalChance;
        clonedInfo.startingWeaponInfos = startingWeaponInfos;

        return clonedInfo;
    }
}