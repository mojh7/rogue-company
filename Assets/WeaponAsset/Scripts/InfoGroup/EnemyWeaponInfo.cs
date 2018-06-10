using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWeaponInfo", menuName = "WeaponData/OwnerEnemy/EnemyWeaponInfo", order = 0)]
public class EnemyWeaponInfo : WeaponInfo
{
    public EnemyWeaponInfo()
    {
        ownerType = OwnerType.Enemy;
    }
}