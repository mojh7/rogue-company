using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SubWeaponAttackType
{
    NON_TARGETING_AUTO_ATTACK,
    TARGETING_AUTO_ATTACK,
    AUTO_TARGETING_AUTO_ATTACK,
    NON_TARGETING_WITH_MAIN_WEAPON,
    TARGETING_WITH_MAIN_WEAPON,
    AUTO_TARGETING_WITH_MAIN_WEAPON
}


//[CreateAssetMenu(fileName = "SubWeaponInfo", menuName = "WeaponData/OwnerPlayer/SubWeaponInfo", order = 1)]
public class SubWeaponInfo : WeaponInfo
{
    //[Header("SubWeapon")]
    //public float angleSpeed;
    //public float radius;
    //public SubWeaponAttackType subWeaponAttackType;
}
