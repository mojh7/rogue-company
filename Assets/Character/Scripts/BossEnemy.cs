using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{

    public override float Attacked(TransferBulletInfo transferBulletInfo)
    {        
        float damage = base.Attacked(transferBulletInfo);
        UIManager.Instance.bossHPUI.DecreaseHp(damage);
        return damage;
    }

    protected override void Die()
    {
        pState = State.DIE;
        EnemyManager.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
        Destroy(this.gameObject);
        DropItem();
    }
}
