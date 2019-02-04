using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletReCollisionManager : MonoBehaviourSingleton<BulletReCollisionManager>
{
    public IEnumerator BecomesPossibleToCollide(Collider2D collider1, Collider2D collider2, float delay)
    {
        //Debug.Log(Time.time + ", " + collider1 + ", " + collider2);
        yield return YieldInstructionCache.WaitForSeconds(delay);
        Physics2D.IgnoreCollision(collider1, collider2, false);
        //Debug.Log(Time.time);
    }
}