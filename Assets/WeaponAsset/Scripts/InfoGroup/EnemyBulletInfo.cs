using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBulletInfo", menuName = "WeaponData/OwnerEnemy/EnemyBulletInfo", order = 1)]
public class EnemyBulletInfo : BulletInfo
{
    public EnemyBulletInfo()
    {
        ownerType = OwnerType.Enemy;
    }
}
