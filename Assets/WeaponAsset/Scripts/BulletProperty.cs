using UnityEngine;
using System.Collections;
using BulletData;
using DelegateCollection;
// BulletProperty.cs

[System.Serializable]
public abstract class BulletProperty
{
    protected GameObject bulletObj;
    protected Bullet bullet;
    protected Transform bulletTrasnform;
    protected DelDestroyBullet delDestroyBullet;
    protected DelCollisionBullet delCollisionBullet;

    public abstract void Init(Bullet bullet);
    //protected WeaponState.Owner owner;
}

/* CollisionProperty class
 * 총알 충돌에 관련된 클래스
 * [현재]
 *  # 충돌 클래스와 삭제 클래스 분리
 *   - 충돌 후 총알이 삭제 되었을 때 내용이 충돌 클래스에 포함되어 있었으나
 *     따로 DeleteProperty를 만들어서 분리 시킴. (DelDestroyBullet)델리게이트로 bullet class에 삭제 함수를 받아서 실행.
 * 
 * 1. BaseNormalCollisionProperty
 *  - 기본 충돌, 1회 충돌 시 삭제
 * 2. LaserCollisionProperty
 *  - 레이저 전용 충돌 속성, 레이저 update 속성에서 raycast 검사를 해서 나온 collider에 대해서 공격 처리 같은 충돌 처리를 담당함.
 * -------------------
 * [예정]
 * 1. BaseNormalProperty에 N회 충돌 시 삭제 로 바꿀까함 == (관통 이랑 비슷, 값이 1이면 관통X, 1 초과면 그 만큼 관통)
 * 
 * [미정]
 * 1.
 */

#region CollisionProperty
public abstract class CollisionProperty : BulletProperty
{
    protected Collider2D bulletCollider2D;
    //public abstract void Init(GameObject bulletObj, Bullet bullet);
    public void Ignore(ref Collision2D coll)
    {
        Physics2D.IgnoreCollision(coll.gameObject.GetComponent<Collider2D>(), bulletCollider2D);
    }
    public void Ignore(ref Collider2D coll)
    {
        Physics2D.IgnoreCollision(coll, bulletCollider2D);
    }
    public abstract CollisionProperty Clone();
    public abstract void Collision(ref Collision2D coll);
    public abstract void Collision(ref Collider2D coll);

}

// 일반 충돌
class BaseNormalCollisionProperty : CollisionProperty
{
    // 관통 관련 변수인데 나중에 관통 속성을 따로 만들던지 그냥 여기서 관통 맡을 수도 있음.
    private int count = 1;

    public override CollisionProperty Clone()
    {
        return new BaseNormalCollisionProperty();
    }

    public override void Collision(ref Collision2D coll)
    {
        Ignore(ref coll);
        if (coll.transform.CompareTag("Wall"))
        {
            if (count == 1)
            {
                // 총알 회수
                delDestroyBullet();
                // 충돌한 충돌체가 몬스터 일 경우 몬스터 피격
                // coll.Attacked();
            }
        }        
    }

    public override void Collision(ref Collider2D coll)
    {
        Ignore(ref coll);
        if (coll.transform.CompareTag("Wall"))
        {
            if (count == 1)
            {
                // 총알 회수
                delDestroyBullet();
                // 충돌한 충돌체가 몬스터 일 경우 몬스터 피격
                // coll.Attacked();
            }
        }
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        bulletObj = bullet.gameObject;
        bulletCollider2D = bullet.boxCollider;
        delDestroyBullet = bullet.DestroyBullet;
        //count = 1;
    }
}

// 레이저 전용 충돌

class LaserCollisionProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new LaserCollisionProperty();
    }

    public override void Collision(ref Collision2D coll)
    {
        // enamy or Player Attack
        // coll.Attacked();
    }

    public override void Collision(ref Collider2D coll)
    {
        // enamy or Player Attack
        // coll.Attacked();
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
    }
}

#endregion

/* UpdateProperty class
 * 총알 행동, 정보 등 매 프레임 별 update에 관련된 클래스
 * [현재]
 * 1. 직선 이동
 * -------------------
 * [예정]
 * 1. 추적 이동
 * 
 * [미정]
 * 1.
 */



/* UpdateProperty class
 * 총알 Update에 관련된 클래스
 * [현재]
 * 1. StraightMoveProperty
 *  - 총알 정해진 방향으로 일정한 속력으로 직선 운동
 *  - 사거리 개념(range)이 있어서 총알 사정거리 넘어서면 delegate로 받아온 총알 삭제 함수 실행.
 *  
 * 2. LaserUpdateProperty
 *  - 레이저 전용 update 속성으로 현재는 raycast 밖에 안함.
 * -------------------
 * [예정]
 * 1. LaserUpdateProperty 에서 레이저 sprite, material, color 이런 외형적인거나 레이저 폭 등등 추가 할게 많이 남아있음.
 * 
 * [미정]
 * 1.
 */


#region UpdateProperty
public abstract class UpdateProperty : BulletProperty
{
    
    public abstract UpdateProperty Clone();
    public abstract void Update();
}

// 기본 직선 운동
public class StraightMoveProperty : UpdateProperty
{
    private float moveSpeed;
    private float currentMoveDistance;
    private float range;

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        bulletTrasnform = bullet.objTransform;
        delDestroyBullet = bullet.DestroyBullet;
        if (bullet.info.speed != 0)
        {
            moveSpeed = bullet.info.speed;
        }
        if (bullet.info.range != 0)
        {
            range = bullet.info.range;
        }
        currentMoveDistance = 0f;
    }

    public override UpdateProperty Clone()
    {
        return new StraightMoveProperty();
    }

    public override void Update()
    {
        bulletTrasnform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        currentMoveDistance += moveSpeed * Time.deltaTime;
        // 이동 범위 넘으면 총알 삭제
        if(currentMoveDistance > range)
        {
            delDestroyBullet();
        }
    }
}

// laser bullet Update
public class LaserUpdateProperty : UpdateProperty
{
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    private float addDirVecMagnitude;

    private LineRenderer lineRenderer;
    private RaycastHit2D hit;
    private int layerMask;
    private Vector3 pos;

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        bulletTrasnform = bullet.objTransform;
        delCollisionBullet = bullet.CollisionBullet;
        ownerDirVec = bullet.GetOwnerDirVec();
        ownerPos = bullet.GetOwnerPos();
        addDirVecMagnitude = bullet.GetAddDirVecMagnitude();
        lineRenderer = bullet.GetLineRenderer();
        pos = new Vector3();
        layerMask = 1 << LayerMask.NameToLayer("Wall");
    }

    public override UpdateProperty Clone()
    {
        return new LaserUpdateProperty();
    }

    public override void Update()
    {
        bulletTrasnform.position = ownerPos();
        pos = ownerPos() + (ownerDirVec() * addDirVecMagnitude);
        // 100f => 레이저에도 사정거리 개념을 넣게 된다면 이 부분 값을 변수로 처리할 예정이고 현재는 일단 raycast 체크 범위를 100f까지 함
        hit = Physics2D.Raycast(pos, ownerDirVec(), 100f, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Wall")) 
        {
            lineRenderer.SetPosition(0, pos);
            lineRenderer.SetPosition(1, hit.point);
            delCollisionBullet(hit.collider);
        }
    }
}
#endregion 

/**/

/* DeleteProperty class
 * 총알이 삭제(회수) 될 때에 관련된 클래스
 * [현재]
 * 1. BaseDeleteProperty
 *  - 일반적인 삭제 속성, 현재는 오브젝트 풀에서 bullet 오브젝트를 회수하는 일만 함.
 * 2. LaserDeleteProperty
 *  - 레이저 전용 삭제 속성, 원래 레이저 총알이면 따로 그때마다 라인 렌더러 컴포넌트 붙인다거나 추가 처리를 할 것 같아서
 *  - 그것에 따른 소멸자 개념으로 처리하려고 따로 빼놓았는데 아마 구현 예정상 bullet 안에다가 라인 렌더러, 트레일 렌더러, 각종 컬라이더 다 넣어놓고
 *  - 총알 종류에 따라서 enable만 true or false 해서 관리 할 것 같아서 레이저 삭제 속성이 현재 따로 하는 일은 없음
 * -------------------
 * [예정]
 * 1. 
 * 
 * [미정]
 * 1. 
 */

#region DeleteProperty
public abstract class DeleteProperty : BulletProperty
{
    public abstract DeleteProperty Clone();
    public abstract void DestroyBullet();

}

public class BaseDeleteProperty : DeleteProperty
{
    public override DeleteProperty Clone()
    {
        return new BaseDeleteProperty();
    }

    public override void DestroyBullet()
    {
        ObjectPoolManager.Instance.DeleteObj(ObjPoolType.Bullet, bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        this.bulletObj = bullet.gameObject;
    }
}

public class LaserDeleteProperty : DeleteProperty
{
    //private LineRenderer lineRenderer;

    public override DeleteProperty Clone()
    {
        return new LaserDeleteProperty();
    }

    public override void DestroyBullet()
    {
        
        ObjectPoolManager.Instance.DeleteObj(ObjPoolType.Bullet, bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        bulletObj = bullet.gameObject;
        //lineRenderer = bullet.GetLineRenderer();
    }
}

//
public class DeleteAfterSummonProperty : DeleteProperty
{
    private BulletPattern bulletPattern;

    public override DeleteProperty Clone()
    {
        return new DeleteAfterSummonProperty();
    }

    public override void DestroyBullet()
    {
        ObjectPoolManager.Instance.DeleteObj(ObjPoolType.Bullet, bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        this.bulletObj = bullet.gameObject;
    }
}

//
 
#endregion


/**/
