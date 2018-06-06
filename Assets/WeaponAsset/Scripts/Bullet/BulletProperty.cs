using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WeaponAsset;
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
    public virtual void Init(Bullet bullet)
    {
        this.bullet = bullet;
        this.bulletObj = bullet.gameObject;
        this.bulletTransform = bullet.objTransform;
        delDestroyBullet = bullet.DestroyBullet;
    }
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

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        bulletCollider2D = bullet.boxCollider;
    }
}

// 일반 충돌
class BaseNormalCollisionProperty : CollisionProperty
{
    private Vector3 reflectVector;

    // 공격이 가능한 오브젝트에 대한 관통 가능 횟수
    private int pierceCount = 1;    // default 1

    // 공격이 가능하지 않은 오브젝트에 대해서 총알이 반사각으로 튕겨나오는 횟수
    private int bounceCount = 0;    // default 0

    public override CollisionProperty Clone()
    {
        return new BaseNormalCollisionProperty();
    }

    // collision
    public override void Collision(ref Collision2D coll)
    {
        // 0531 근접 무기 총알 블락 땜빵 코드
        if (OwnerType.Enemy == bullet.GetOwnerType() && coll.transform.CompareTag("PlayerBlockBullet"))
        {
            delDestroyBullet();
        }


        // 공격 가능 object, 관통 횟수 == 1 이면 총알 delete 처리
        if (OwnerType.Player == bullet.GetOwnerType() && coll.transform.CompareTag("Enemy"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);

            Ignore(ref coll);

            // 관통 횟수 -1
            pierceCount -= 1;

            if (pierceCount == 0)
            {
                // 총알 delete 처리
                delDestroyBullet();
            }
        }
        else if(OwnerType.Enemy == bullet.GetOwnerType() && coll.transform.CompareTag("Player"))
        {
            Debug.Log("Player 피격 Collsion");
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);

            Ignore(ref coll);

            // 관통 횟수 -1
            pierceCount -= 1;

            if (pierceCount == 0)
            {
                // 총알 delete 처리
                delDestroyBullet();
            }
        }
        // 공격 불가능 object, bounce 횟수 == 0 이면 총알 delete 처리
        else if (coll.transform.CompareTag("Wall"))
        {
            // bounce 가능 횟수가 남아있으면 총알을 반사각으로 튕겨내고 없으면 delete 처리
            if (bounceCount > 0)
            {
                //Debug.Log("bounceCount : " + bounceCount);
                // 총알 반사각으로 bounce

                //반사각
                reflectVector = Vector3.Reflect(MathCalculator.VectorRotate(Vector3.right, bulletTransform.rotation.eulerAngles.z), coll.contacts[0].normal);

                ///Debug.Log("normal Vector : " + coll.contacts[0].normal);
                ///Debug.Log("입사각 : " + MathCalculator.VectorRotate(Vector3.right, bulletTransform.rotation.eulerAngles.z));
                ///Debug.Log("반사각 : " + reflectVector.GetDegFromVector());
                bullet.SetDirection(reflectVector);
                bounceCount -= 1;

                // 디버그용 contact 위치 표시
                ///TestScript.Instance.CreateContactObj(coll.contacts[0].point);

            }
            else
            {
                // 총알 회수
                delDestroyBullet();
            }
        }
    }

    // trigger
    public override void Collision(ref Collider2D coll)
    {
        // 0531 근접 무기 총알 블락 땜빵 코드
        if (OwnerType.Enemy == bullet.GetOwnerType() && coll.CompareTag("PlayerBlockBullet"))
        {
            delDestroyBullet();
        }

        // 공격 가능 object, 관통 횟수 == 1 이면 총알 delete 처리
        if (OwnerType.Player == bullet.GetOwnerType() && coll.CompareTag("Enemy"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);

            Ignore(ref coll);

            // 관통 횟수 -1
            pierceCount -= 1;

            if (pierceCount == 0)
            {
                // 총알 delete 처리
                delDestroyBullet();
            }
        }
        else if (OwnerType.Enemy == bullet.GetOwnerType() && coll.CompareTag("Player"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);

            Ignore(ref coll);

            // 관통 횟수 -1
            pierceCount -= 1;

            if (pierceCount == 0)
            {
                // 총알 delete 처리
                delDestroyBullet();
            }
        }

        // 공격 불가능 object, bounce 횟수 == 0 이면 총알 delete 처리
        else if (coll.CompareTag("Wall"))
        {
            // bounce 가능 횟수가 남아있으면 총알을 반사각으로 튕겨내고 없으면 delete 처리
            if (bounceCount > 0)
            {
                // 총알 반사각으로 bounce
                //incomingVector = MathCalculator.RotateRadians(Vector3.right, bulletTransform.rotation.eulerAngles.z);
                //normalVector = (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized;

                Debug.Log("Trigger");

                //bullet.GetDirVector()
                //반사각
                reflectVector = Vector3.Reflect(MathCalculator.VectorRotate(Vector3.right, bulletTransform.rotation.eulerAngles.z), (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized);
                //Debug.Log("입사각 : " + bulletTransform.rotation.eulerAngles.z + ", 반사각 : " + reflectVector.GetDegFromVector() + ",conut : " + bounceCount + ", normal : " + (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized);
                bullet.SetDirection(reflectVector);
                
                bounceCount -= 1;

                // TestScript.Instance.CreateEffect(bulletTransform.position);
                // 디버그용 contact 위치 표시
                TestScript.Instance.CreateContactObj(coll.bounds.ClosestPoint(bulletTransform.position));

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
        base.Init(bullet);
        reflectVector = new Vector3();

        pierceCount = bullet.info.pierceCount;
        bounceCount = bullet.info.bounceCount;
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
        // coll.attack
    }

    public override void Collision(ref Collider2D coll)
    {
        if (OwnerType.Player == bullet.GetOwnerType() && coll.CompareTag("Enemy"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage * Time.fixedDeltaTime, bullet.info.knockBack, bullet.info.criticalRate);
        }
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
    }
}

// 삭제되지 않은 충돌 속성, lifeTime에 의한 삭제만 이루어짐
class UndeletedCollisionProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new UndeletedCollisionProperty();
    }

    // collision
    public override void Collision(ref Collision2D coll)
    {
        if (OwnerType.Player == bullet.GetOwnerType() && coll.transform.CompareTag("Enemy"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);
            Ignore(ref coll);
        }
        else if (OwnerType.Enemy == bullet.GetOwnerType() && coll.transform.CompareTag("Player"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);
            Ignore(ref coll);
        }
    }

    // trigger
    public override void Collision(ref Collider2D coll)
    {
        // 공격 가능 object, 관통 횟수 == 1 이면 총알 delete 처리
        if (OwnerType.Player == bullet.GetOwnerType() && coll.CompareTag("Enemy"))
        {
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);
            Ignore(ref coll);
        }
        else if (OwnerType.Enemy == bullet.GetOwnerType() && coll.CompareTag("Player"))
        {
            // 공격 처리
            coll.gameObject.GetComponent<Character>().Attacked(bullet.GetDirVector(), bulletTransform.position, bullet.info.damage, bullet.info.knockBack, bullet.info.criticalRate, bullet.info.positionBasedKnockBack);
            Ignore(ref coll);
        }
 
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
    }
}
// baseNormalCollsion에 bounce충돌 같이 있는데 따로 빼놓을 예정

/*
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
        incomingVector = MathCalculator.RotateRadians(Vector3.right, bulletTransform.rotation.eulerAngles.z);
        normalVector = (bulletTransform.position - coll.bounds.ClosestPoint(bulletTransform.position)).normalized;
        reflectVector = Vector3.Reflect(incomingVector, normalVector); //반사각

        Debug.Log("입사각 : " + bulletTransform.rotation.eulerAngles.z);
        bulletTransform.rotation = Quaternion.Euler(0, 0, reflectVector.GetDegFromVector());
        Debug.Log("반사각 : " + bulletTransform.rotation.eulerAngles.z);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
    }
}
*/
#endregion


/* initializationProperty
 * bullet이 오브젝트 풀에서 생성되어서 init 함수에서 초기화 될 때 최초 한 번만 실행되는 속성들
 * 
 * 
 */




/* UpdateProperty class
 * 총알 Update에 관련된 클래스
 * 
 * 원래는 bullet class에서 코루틴으로 돌아서 60fps 정도로 실행하게 하려 했으나 현재 일단은 fixedUpdate에서 updateProperty를 실행하게 함.
 * 
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

/// <summary>
/// 등속 직선 운동 속성
/// </summary>
public class StraightMoveProperty : UpdateProperty
{
    private float moveSpeed;    // 속력
    private float range;

    private float lifeTime;  // bullet 생존 시간 * (1.0f / Time.fixedDeltaTime)
    private float timeCount; // 지나간 시간 카운트

    public override UpdateProperty Clone()
    {
        return new StraightMoveProperty();
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        timeCount = 0;
        if (bullet.info.speed != 0)
        {
            moveSpeed = bullet.info.speed;
        }
        if (bullet.info.range != 0)
        {
            range = bullet.info.range;
        }

        lifeTime = (range / moveSpeed);
    }

    // range / moveSpeed 시간 지나면 삭제
    public override void Update()
    {
        if(timeCount >= lifeTime)
        {
            delDestroyBullet();
        }
        timeCount += Time.fixedDeltaTime;
    }
}

/// <summary> 등가속도 직선? 운동 </summary>
public class AccelerationMotionProperty : UpdateProperty
{
    private float distance;     // 이동한 거리, range 체크용

    private float moveSpeed;    // 속력
    private float acceleration; // 가속도 (방향 일정)
    private float range;        // 사정거리
    // 속력이 변화하는 총 값 제한, ex) a = -1, limit = 10, 속력 v = 3-> -7까지만 영향받음. a = +2 limit 8, v = -2 => +6까지만
    private float deltaSpeedTotal;
    private float deltaSpeedTotalLimit;

    private bool acceleratesBullet;     // 가속도를 적용 할 것인가 말 것인가.

    private float deltaSpeed;

    public override UpdateProperty Clone()
    {
        return new AccelerationMotionProperty();
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        distance = 0;
        deltaSpeedTotal = 0;
        deltaSpeedTotalLimit = bullet.info.deltaSpeedTotalLimit;
        
        if (bullet.info.speed != 0)
        {
            moveSpeed = bullet.info.speed;
        }
        if (bullet.info.acceleration != 0)
        {
            acceleration = bullet.info.acceleration;
        }
        if (bullet.info.range != 0)
        {
            range = bullet.info.range;
        }

        

        acceleratesBullet = true;
    }

    public override void Update()
    {
        // 이동
        bullet.SetVelocity(moveSpeed);
        distance += moveSpeed * Time.fixedDeltaTime;

        // 사정거리 넘어가면 delete 속성 실행
        if(distance >= range)
        {
            delDestroyBullet();
        }

        // 가속화
        if(acceleratesBullet)
        {
            deltaSpeed = acceleration * Time.fixedDeltaTime;
            moveSpeed += deltaSpeed;
            deltaSpeedTotal += Mathf.Abs(deltaSpeed);

            // 속력 변한 총량이 limit 보다 커지면 가속도 적용 멈춤.
            if (deltaSpeedTotal >= deltaSpeedTotalLimit)
            {
                acceleratesBullet = false;
                if (acceleration > 0)
                {
                    moveSpeed -= deltaSpeedTotal - deltaSpeedTotalLimit;
                }
                else if (acceleration < 0)
                {
                    moveSpeed += deltaSpeedTotal - deltaSpeedTotalLimit;
                }

                if(Mathf.Abs(moveSpeed) < 0.1f)
                {
                    moveSpeed = 0f;
                }
            }

            // 속력이 음수가 되며 방향 자체가 바뀔 때
            if (moveSpeed < 0)
            {
                moveSpeed = -moveSpeed;
                acceleration = -acceleration;
                bullet.RotateDirection(180);
            }
        }
    }
}


/// <summary> laser bullet Update </summary>
public class LaserUpdateProperty : UpdateProperty
{
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    private float addDirVecMagnitude;

    private LineRenderer lineRenderer;
    private RaycastHit2D hit;
    private int layerMask;
    private Vector3 pos;

    private bool AttackAble;

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);


        delCollisionBullet = bullet.CollisionBullet;

        ownerDirVec = bullet.GetOwnerDirVec();
        ownerPos = bullet.GetOwnerPos();
        addDirVecMagnitude = bullet.GetAddDirVecMagnitude();
        lineRenderer = bullet.GetLineRenderer();
        pos = new Vector3();
        // 일단 Player 레이저가 Enemy에게 적용 하는 것만
        layerMask = 1 << LayerMask.NameToLayer("Wall");
        layerMask |= 1 << LayerMask.NameToLayer("Enemy");
    }

    public override UpdateProperty Clone()
    {
        return new LaserUpdateProperty();
    }

    public override void Update()
    {
        bulletTransform.position = ownerPos();
        pos = ownerPos() + (ownerDirVec() * addDirVecMagnitude);
        pos.z = 0;
        bullet.LaserStartPoint.position = pos;
        // 100f => 레이저에도 사정거리 개념을 넣게 된다면 이 부분 값을 변수로 처리할 예정이고 현재는 일단 raycast 체크 범위를 100f까지 함
        hit = Physics2D.Raycast(pos, ownerDirVec(), 100f, layerMask);
        if (hit.collider != null && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Enemy"))) 
        {
            lineRenderer.SetPosition(0, pos);
            lineRenderer.SetPosition(1, hit.point);
            bullet.LaserEndPoint.position = hit.point;
            delCollisionBullet(hit.collider);
        }
    }
}

/// <summary> 일정 텀(creationCycle)을 가지고 bulletPattern대로 총알을 소환하는 속성 </summary>
public class SummonProperty : UpdateProperty
{
    private BulletPattern bulletPattern; // 생성할 총알 패턴
    private float creationCycle; // 생성 주기
    private float timeCount; // time count
    
    private DelGetDirDegree bulletDirDegree;
    private DelGetPosition bulletDirVec;
    private DelGetPosition bulletPos;

    public SummonProperty(BulletPattern bulletPattern, float creationCycle)
    {
        this.bulletPattern = bulletPattern;
        this.creationCycle = creationCycle;
    }
    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        
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
        if (timeCount >= creationCycle)
        {
            timeCount -= creationCycle;
            bulletPattern.CreateBullet(1.0f);
        }
        timeCount += Time.fixedDeltaTime;
    }
}

/// <summary> 유도 총알 </summary>
public class HomingProperty : UpdateProperty
{
    private float lifeTime;
    private float timeCount;
    private float deltaAngle;
    private int enemyTotal;
    private float targettingCycle;

    private RaycastHit2D hit;
    private List<RaycasthitEnemy> raycastHitEnemies;
    private RaycasthitEnemy raycasthitEnemyInfo;
    private int layerMask;
    private Transform enemyTransform;

    private Vector3 directionVector;
    private float directionDegree;
    private Vector3 differenceVector;
    private float differenceDegree;

    int raycasthitEnemyNum = 0;
    float minDistance = 1000f;
    int proximateEnemyIndex = -1;

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);

        targettingCycle = 0;
        deltaAngle = 4f;

        directionVector = new Vector3(0f, 0f, 0f);
        differenceVector = new Vector3(0f, 0f, 0f);

        raycastHitEnemies = new List<RaycasthitEnemy>();
        raycasthitEnemyInfo = new RaycasthitEnemy();
        // 막히는 장애물 리스트 더 추가시 Wall 말고 더 넣어야됨.
        layerMask = 1 << LayerMask.NameToLayer("Wall");

        timeCount = 0;
        if (bullet.info.speed != 0)
        {
            lifeTime = bullet.info.range / bullet.info.speed;
        }
        else
            lifeTime = 20;
        
    }

    public override UpdateProperty Clone()
    {
        return new HomingProperty();
    }

    public override void Update()
    {
        if (timeCount >= lifeTime)
        {
            delDestroyBullet();
        }
        timeCount += Time.fixedDeltaTime;
        //targettingCycle -= Time.fixedDeltaTime;
        // 유도 대상 선정

        enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();

        List<Enemy> enemyList = EnemyManager.Instance.GetEnemyList;

        raycastHitEnemies.Clear();
        raycasthitEnemyNum = 0;
        minDistance = 1000f;
        proximateEnemyIndex = -1;

        // raycast로 bullet과 enemy 사이에 장애물이 없는 enmey 방향만 찾아낸다.
        for (int i = 0; i < enemyTotal; i++)
        {
            raycasthitEnemyInfo.index = i;
            raycasthitEnemyInfo.distance = Vector2.Distance(enemyList[i].transform.position, bulletTransform.position);
            hit = Physics2D.Raycast(bulletTransform.position, enemyList[i].transform.position - bulletTransform.position, raycasthitEnemyInfo.distance, layerMask);
            if (hit.collider == null)
            {
                raycastHitEnemies.Add(raycasthitEnemyInfo);
                raycasthitEnemyNum += 1;
            }
        }

        // 위에서 찾은 enmey들 중 distance가 가장 작은, 가장 가까운 enemy를 찾는다.
        for (int j = 0; j < raycasthitEnemyNum; j++)
        {
            if (raycastHitEnemies[j].distance <= minDistance)
            {
                minDistance = raycastHitEnemies[j].distance;
                proximateEnemyIndex = j;
            }
        }

        // 선정된 대상에게 유도 될 수 있도록 회전
        if (proximateEnemyIndex != -1)
        {
            enemyTransform = enemyList[raycastHitEnemies[proximateEnemyIndex].index].transform;

            differenceVector = enemyTransform.position - bulletTransform.position;
            // (Bx-Ax)*(Py-Ay) - (By-Ay)*(Px-Ax)
            if ( ((bullet.GetDirVector().x) * (differenceVector.y) - (bullet.GetDirVector().y) * (differenceVector.x)) >= 0 )
            {
                bullet.RotateDirection(deltaAngle);
            }
            else if ((bullet.GetDirVector().x) * (differenceVector.y) - (bullet.GetDirVector().y) * (differenceVector.x) < 0)
            {
                bullet.RotateDirection(-deltaAngle);
            }
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

/// <summary>
/// 기본 총알 삭제, effect만 생성
/// </summary>
public class BaseDeleteProperty : DeleteProperty
{
    public override DeleteProperty Clone()
    {
        return new BaseDeleteProperty();
    }

    public override void DestroyBullet()
    {
        // ObjectPoolManager.Instance.CreateEffect(bullet.info.effectId, bulletTransform.position);
        TestScript.Instance.CreateEffect(bullet.info.effectId, bulletTransform.position);
        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
    }
}

/// <summary>
/// 레이저 전용 삭제 속성
/// </summary>
public class LaserDeleteProperty : DeleteProperty
{
    //private LineRenderer lineRenderer;

    public override DeleteProperty Clone()
    {
        return new LaserDeleteProperty();
    }

    public override void DestroyBullet()
    {

        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        //lineRenderer = bullet.GetLineRenderer();
    }
}

// DeleteAfterSummonBullet
// DeleteAfterSummonPattern

/// <summary>
/// 본래 총알이 삭제 될 때 새로운 bullet이 똑같은 position에 생성됨
/// 수류탄, 로켓런쳐 같은 본래 bullet이 터지고 폭발하는 총알에 쓰일 delete 속성
/// </summary>
public class DeleteAfterSummonBulletProperty : DeleteProperty
{
    private GameObject createdObj;

    public override DeleteProperty Clone()
    {
        return new DeleteAfterSummonBulletProperty();
    }

    public override void DestroyBullet()
    {
        createdObj = ObjectPoolManager.Instance.CreateBullet();
        createdObj.GetComponent<Bullet>().Init(bullet.info.deleteAfterSummonBulletInfo.Clone(), bullet.GetOwnerType(), bulletTransform.position);
        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        bullet.info.deleteAfterSummonBulletInfo.Init();
    }
}

/// <summary>
/// 총알 삭제시 bulletPattern 생성 후 삭제
/// 현재는 multiPattern만 1회 생성으로 되어있고 추후 필요 시
/// SummonUpdate 속성 처럼 다양한 패턴과 횟수의 pattern을 생성할 수도 있음.
/// </summary>
public class DeleteAfterSummonPatternProperty : DeleteProperty
{
    private BulletPattern summonBulletPattern;
    

    public override DeleteProperty Clone()
    {
        return new DeleteAfterSummonPatternProperty();
    }

    public override void DestroyBullet()
    {
        // 일단 임시로 1.0f 추 후 buff에서 받아온 데미지 뻥튀기 만큼 값 조절
        summonBulletPattern.StartAttack(1.0f, bullet.GetOwnerType());
        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        summonBulletPattern = new MultiDirPattern(bullet.info.deleteAfterSummonPatternInfo as MultiDirPatternInfo, 1, 0, bullet.GetOwnerType());
    }
}

//

#endregion


/**/