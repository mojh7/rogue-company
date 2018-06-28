using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    public override void Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalRate, bool positionBasedKnockBack = false)
    {
        base.Attacked(_dir, bulletPos, damage, knockBack, criticalRate, positionBasedKnockBack);
        UIManager.Instance.bossHPUI.DecreaseHp(damage); 
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
