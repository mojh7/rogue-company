using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 // armor 쉴드 개념 있는 아이템들 있어서 아예 player 데미지 입는 공식
 // 데미지 - 방어력으로 하고 저런 아이템 먹으면 방어력 수치 잠깐 올렸다가 내릴려고 함.

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "CharData/PlayerData")]
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
    private float hpMax;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private int stamina;
    [SerializeField]
    private int staminaMax;
    [SerializeField]
    private float skillGauge;
    [SerializeField]
    private int skillGaugeMax;
    [SerializeField]
    private float criticalChance;
    [SerializeField]
    private int[] startingWeaponInfos = new int[3];
    [SerializeField]
    private SkillData skillData;

    private int shield;

    // on / off bool;
    private bool canDrainHp;
    private bool cannotDamagedWhenFalling;
    #endregion

    #region get/set Property

    public float Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    public float HpMax
    {
        get { return hpMax; }
        set { hpMax = value; }
    }
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    public int Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }
    public int StaminaMax
    {
        get { return staminaMax; }
        set { staminaMax = value; }
    }

    public float SkillGauge
    {
        get { return skillGauge; }
        set { skillGauge = value; }
    }
    public int SkillGaugeMax
    {
        get { return skillGaugeMax; }
        set { skillGaugeMax = value; }
    }

    public int Shield
    {
        get { return shield; }
        set { shield = value; }
    }
    public float CriticalChance
    {
        get { return criticalChance; }
        set { criticalChance = value; }
    }
    public int[] StartingWeaponInfos
    {
        get { return startingWeaponInfos; }
    }
    public SkillData SkillData
    {
        get
        {
            return skillData;
        }
    }
    #endregion

    public PlayerData Clone()
    {
        PlayerData clonedInfo = CreateInstance<PlayerData>();

        clonedInfo.playerType = playerType;
        clonedInfo.playerSprite = playerSprite;
        clonedInfo.hp = hp;
        clonedInfo.hpMax = hpMax;
        clonedInfo.moveSpeed = moveSpeed;
        clonedInfo.stamina = stamina;
        clonedInfo.staminaMax = staminaMax;
        clonedInfo.skillGauge = skillGauge;
        clonedInfo.skillGaugeMax = skillGaugeMax;
        clonedInfo.shield = shield;
        clonedInfo.criticalChance = criticalChance;
        clonedInfo.startingWeaponInfos = startingWeaponInfos;

        clonedInfo.canDrainHp = canDrainHp;
        clonedInfo.cannotDamagedWhenFalling = cannotDamagedWhenFalling;

        clonedInfo.skillData = skillData;
        return clonedInfo;
    }
}