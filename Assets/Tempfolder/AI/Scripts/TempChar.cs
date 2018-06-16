using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TempChar : Character {

    Vector2 target;
    bool stop;

    public override void Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalRate, bool positionBasedKnockBack = false)
    {
        throw new System.NotImplementedException();
    }
    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
    public void Move(Vector2 target)
    {
        Debug.DrawLine(transform.position, target, Color.black);
    }


}
