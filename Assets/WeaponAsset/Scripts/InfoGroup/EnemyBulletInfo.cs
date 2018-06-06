using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBulletInfo", menuName = "EnemyWeaponData/EnemyBulletInfo", order = 1)]
public class EnemyBulletInfo : BulletInfo {
    public EnemyBulletInfo()
    {
        ownerType = OwnerType.Enemy;
    }
}
