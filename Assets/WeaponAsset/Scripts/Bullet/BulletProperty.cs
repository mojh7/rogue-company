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
    protected Transform bulletTransform;
    protected DelDestroyBullet delDestroyBullet;
    protected DelCollisionBullet delCollisionBullet;

    /// <summary>
    /// bullet class에 정보를 받아와서 속성에 맞는 초기화
    /// </summary>
    /// <param name="bullet"></param>
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
 *  
 * 3. BounceCollisionProperty
 *  - 몬스터 충돌이 아닌 튕겨나갈 오브젝트 들(벽, 다른 고정된 오브젝트 등)과 충돌시 반사각으로 튕겨나감
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
    private Vector3 reflectVector;

    // 공격이 가능한 오브젝트에 대한 관통 가능 횟수
    private int pierceCount = 1;    // default 1

    // 공격이 가능하지 않은 오브젝트에 대해서 총알이 반사각으로 튕겨나오는 횟수
    private int bounceCount = 3;    // default 0

    public override CollisionProperty Clone()
    {
        return new BaseNormalCollisionProperty();
    }

    public override void Collision(ref Collision2D coll)
    {     
    }

    public override void Collision(ref Collider2D coll)
    {
        // 공격 가능 object, 관통 == 1 이면 총알 delete 처리
        if (coll.CompareTag("Enemy"))
        {
            // 공격 처리
            // coll.Attacked();
            Ignore(ref coll);

            // 관통 횟수 -1
            pierceCount -= 1;

            if(pierceCount == 0)
            {
                // 총알 delete 처리
                delDestroyBullet();
            }
        }

        // 공격 가능 object, 관통 == 1 이면 총알 delete 처리
        if (coll.CompareTag("Wall"))
        {
            // bounce 가능 횟수가 남아있으면 총알을 반사각으로 튕겨내고 없으면 delete 처리
            if (bounceCount > 0)
            {
                // 총알 반사각으로 bounce
                //incomingVector = MathCalculator.RotateRadians(Vector3.right, bulletTransform.rotation.eulerAngles.z);
                //normalVector = (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized;
                reflectVector = Vector3.Reflect(MathCalculator.RotateRadians(Vector3.right, bulletTransform.rotation.eulerAngles.z), (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized); //반사각

                Debug.Log("입사각 : " + bulletTransform.rotation.eulerAngles.z + ", 반사각 : " + reflectVector.GetDegFromVector());
                bulletTransform.rotation = Quaternion.Euler(0, 0, reflectVector.GetDegFromVector());
                Debug.Log("남은 bounceConut : " + bounceCount);
                bounceCount -= 1;
            }
            else
            {
                // 총알 회수
                delDestroyBullet();
            }
        }
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        bulletObj = bullet.gameObject;
        bulletTransform = bullet.objTransform;
        bulletCollider2D = bullet.boxCollider;
        delDestroyBullet = bullet.DestroyBullet;
        reflectVector = new Vector3();
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

/// <summary>
/// 
/// </summary>
class BounceCollisionProperty : CollisionProperty
{
    private Vector3 incomingVector;
    private Vector3 normalVector;
    private Vector3 reflectVector;


    public override CollisionProperty Clone()
    {
        return new BounceCollisionProperty();
    }

    public override void Collision(ref Collision2D coll)
    {
        
    }

    public override void Collision(ref Collider2D coll)
    {
        //createdObj = Instantiate(contactPointObj);
        //createdObj.GetComponent<Transform>().position = coll.bounds.ClosestPoint(objTransform.position);
        //

        incomingVector = MathCalculator.RotateRadians(Vector3.right, bulletTransform.rotation.eulerAngles.z);
        normalVector = (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized;
        reflectVector = Vector3.Reflect(incomingVector, normalVector); //반사각

        Debug.Log("입사각 : " + bulletTransform.rotation.eulerAngles.z);
        bulletTransform.rotation = Quaternion.Euler(0, 0, reflectVector.GetDegFromVector());
        Debug.Log("반사각 : " + bulletTransform.rotation.eulerAngles.z);
    }

    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        this.bulletTransform = bullet.objTransform;
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
 *  
 * 3. SummonProperty
 *  - 일정 주기로(현재는 시간 단위이고 거리 단위는 고려) 총알을 따로 더 생성함
 *  - bulletPattern 포함
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
        bulletTransform = bullet.objTransform;
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
        bulletTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
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
        bulletTransform = bullet.objTransform;
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
        bulletTransform.position = ownerPos();
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

// bullet class 에서 60fps로 코루틴으로 update문을 실행하는데 시간주기를 뭘로 체크할지 고민중
public class SummonProperty : UpdateProperty
{
    private BulletPattern bulletPattern; // 생성할 총알 패턴
    private int creationCycle; // 생성 주기
    private int frameCount; // frame count
    private DelGetDirDegree bulletDirDegree;
    private DelGetPosition bulletDirVec;
    private DelGetPosition bulletPos;

    public SummonProperty(BulletPattern bulletPattern, int creationCycle)
    {
        this.bulletPattern = bulletPattern;
        this.creationCycle = creationCycle;
    }
    public override void Init(Bullet bullet)
    {
        this.bullet = bullet;
        bulletTransform = bullet.objTransform;
        bulletDirDegree = bullet.GetDirDegree;
        bulletDirVec = () => { return Vector3.zero; };
        bulletPos = bullet.GetPosition;
        bulletPattern.Init(bulletDirDegree, bulletDirVec, bulletPos);
    }
    public override UpdateProperty Clone()
    {
        return new SummonProperty(bulletPattern.Clone(), creationCycle);
    }

    // 생성 주기마다 bulletPattern 실행
    public override void Update()
    {
        frameCount += 1;
        if (frameCount == creationCycle)
        {
            frameCount = 0;
            bulletPattern.CreateBullet(1.0f);
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
