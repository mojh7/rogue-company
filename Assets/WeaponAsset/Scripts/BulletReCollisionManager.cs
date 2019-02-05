using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollisionProperty 에서 코루틴을 써야되는데 MonoBehaviour가 아니여서 로직 따로 뺐음.

public class BulletReCollisionManager : MonoBehaviourSingleton<BulletReCollisionManager>
{
    /// <summary>
    /// delay초 후 IngoreCollision이 false가 되어 coll1과 coll2가 충돌이 되게끔 함. 
    /// </summary>
    public IEnumerator BecomesPossibleToCollide(Collider2D collider1, Collider2D collider2, float delay)
    {
        //Debug.Log(Time.time + ", " + collider1 + ", " + collider2);
        yield return YieldInstructionCache.WaitForSeconds(delay);
        Physics2D.IgnoreCollision(collider1, collider2, false);
        //Debug.Log(Time.time);
    }
}