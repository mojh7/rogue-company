using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

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
 *  
 * 4. 
 * -------------------
 * [예정]
 * 1. LaserUpdateProperty 에서 레이저 sprite, material, color 이런 외형적인거나 레이저 폭 등등 추가 할게 많이 남아있음.
 * 
 * [미정]
 * 1.
 */


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
        if (timeCount >= lifeTime)
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
        if (distance >= range)
        {
            delDestroyBullet();
        }

        // 가속화
        if (acceleratesBullet)
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

                if (Mathf.Abs(moveSpeed) < 0.1f)
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
    private DelGetPosition ownerPos;
    private DelGetPosition ownerDirVec;
    private DelGetDirDegree ownerDirDegree;
    private float angle;
    private float addDirVecMagnitude;

    private LineRenderer lineRenderer;
    private RaycastHit2D hit;
    private int layerMask;
    private Vector3 pos;
    private Vector2 laserSize;
    private bool AttackAble;

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);

        delCollisionBullet = bullet.CollisionBullet;

        ownerPos = bullet.GetOwnerPos();
        ownerDirVec = bullet.GetOwnerDirVec();
        ownerDirDegree = bullet.GetOwnerDirDegree();
        
        addDirVecMagnitude = bullet.GetAddDirVecMagnitude();
        lineRenderer = bullet.GetLineRenderer();
        pos = new Vector3();
        laserSize = new Vector2(0.2f, 0.2f);
        // 일단 Player 레이저가 Enemy에게 적용 하는 것만
        layerMask = (1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("TransparentFX"));
    }

    public override UpdateProperty Clone()
    {
        return new LaserUpdateProperty();
    }

    public override void Update()
    {
        //Debug.Log(ownerDirDegree());
        bulletTransform.position = ownerPos();
        pos = ownerPos() + (ownerDirVec() * addDirVecMagnitude);
        pos.z = 0;
        bullet.LaserStartPoint.position = pos;
        // 100f => 레이저에도 사정거리 개념을 넣게 된다면 이 부분 값을 변수로 처리할 예정이고 현재는 일단 raycast 체크 범위를 100f까지 함
        hit = Physics2D.BoxCast(pos, laserSize, ownerDirDegree(), ownerDirVec(), 100f, layerMask);
        //hit = Physics2D.Raycast(pos, ownerDirVec(), 100f, layerMask);
        // && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Enemy")
        if (null != hit.collider)
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
        bulletPattern.Init(ownerBuff, transferBulletInfo, bulletDirDegree, bulletDirVec, bulletPos);
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

    private float homingStartTime;
    private float homingEndTime;

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
        homingStartTime = bullet.info.homingStartTime;
        homingEndTime = bullet.info.homingEndTime;

        if (bullet.info.speed != 0)
        {
            lifeTime = bullet.info.range / bullet.info.speed;
        }
        else
        {
            lifeTime = 10f;
        }

    }

    public override UpdateProperty Clone()
    {
        return new HomingProperty();
    }

    public override void Update()
    {
        timeCount += Time.fixedDeltaTime;
        if (timeCount < homingStartTime || homingEndTime < timeCount)
        {
            return;
        }
        if (timeCount >= lifeTime)
        {
            delDestroyBullet();
        }

        // owner 별로 유도 대상이 다름.
        if(CharacterInfo.OwnerType.Player == bullet.GetOwnerType())
        {
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
                if (((bullet.GetDirVector().x) * (differenceVector.y) - (bullet.GetDirVector().y) * (differenceVector.x)) >= 0)
                {
                    bullet.RotateDirection(deltaAngle);
                }
                else if ((bullet.GetDirVector().x) * (differenceVector.y) - (bullet.GetDirVector().y) * (differenceVector.x) < 0)
                {
                    bullet.RotateDirection(-deltaAngle);
                }
            }
        }
        else if (CharacterInfo.OwnerType.Enemy == bullet.GetOwnerType())
        {
            differenceVector = PlayerManager.Instance.GetPlayer().GetPosition() - bulletTransform.position;
            // (Bx-Ax)*(Py-Ay) - (By-Ay)*(Px-Ax)
            if (((bullet.GetDirVector().x) * (differenceVector.y) - (bullet.GetDirVector().y) * (differenceVector.x)) >= 0)
            {
                bullet.RotateDirection(deltaAngle * 0.3f);
            }
            else if ((bullet.GetDirVector().x) * (differenceVector.y) - (bullet.GetDirVector().y) * (differenceVector.x) < 0)
            {
                bullet.RotateDirection(-deltaAngle * 0.3f);
            }
        }
    }
}

/// <summary> 설치 무기 마인화 용 속성 </summary>
public class MineBombProperty : UpdateProperty
{
    private enum MineState { DETECTION, TRAKING }
    private MineState state;
    private float speed;
    private CircleCollider2D detectedEnemy;
    private List<CircleCollider2D> enemies;
    public override UpdateProperty Clone()
    {
        return new MineBombProperty();
    }

    public override void Update()
    {
        // 주변 감지
        if(MineState.DETECTION == state)
        {
            enemies = EnemyManager.Instance.GetEnemyColliderList;

            for (int i = 0; i < enemies.Count; i++)
            {
                Vector2 distance = enemies[i].transform.position - bulletTransform.position;
                if ((distance.x * distance.x + distance.y * distance.y) < 9)
                {
                    detectedEnemy = enemies[i];
                    Debug.Log("추적 시작");
                    state = MineState.TRAKING;
                }
            }
        }
        // 추적
        else if(MineState.TRAKING == state)
        {

            // if(detectedEnemy)

            // 각도 맞춰서 추적

            
            Vector2 dir = (detectedEnemy.transform.position - bulletTransform.position).normalized;
            bullet.info.speed = 3f;
            bullet.SetDirection(new Vector3(dir.x, dir.y, 0));
            

            /*
            // x, y 따로 이동
            if (bulletTransform.position.x - detectedEnemy.transform.position.x > 0.1f
                || bulletTransform.position.x - detectedEnemy.transform.position.x < -0.1f)
            {
                if (bulletTransform.position.x < detectedEnemy.transform.position.x)
                {
                    bulletTransform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
                }
                else
                {
                    bulletTransform.Translate(Vector2.left * speed * Time.fixedDeltaTime);
                }
            }

            if(bulletTransform.position.y - detectedEnemy.transform.position.y > 0.1f
                || bulletTransform.position.y - detectedEnemy.transform.position.y < -0.1f)
            {
                if (bulletTransform.position.y < detectedEnemy.transform.position.y)
                {
                    bulletTransform.Translate(Vector2.up * speed * Time.fixedDeltaTime);
                }
                else
                {
                    bulletTransform.Translate(Vector2.down * speed * Time.fixedDeltaTime);
                }
            }*/
        }
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        state = MineState.DETECTION;
        speed = 2f;
    }
}

public class FixedOwnerProperty : UpdateProperty
{
    private DelGetPosition ownerPos;
    private DelGetPosition ownerDirVec;
    private DelGetDirDegree ownerDirDegree;
    private float angle;
    private float addDirVecMagnitude; 

    private Vector3 pos;

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);

        delCollisionBullet = bullet.CollisionBullet;

        ownerPos = bullet.GetOwnerPos();
        ownerDirVec = bullet.GetOwnerDirVec();
        ownerDirDegree = bullet.GetOwnerDirDegree();

        addDirVecMagnitude = bullet.GetAddDirVecMagnitude();
        pos = new Vector3();
    }

    public override UpdateProperty Clone()
    {
        return new FixedOwnerProperty();
    }

    public override void Update()
    {
        bulletTransform.position = ownerPos() + (ownerDirVec() * addDirVecMagnitude);
        bulletTransform.rotation = Quaternion.Euler(0, 0, ownerDirDegree());
    }
}

/// <summary> 나선형 속성 </summary>
public class SpiralProperty : UpdateProperty
{
    private float lifeTime;
    private float timeCount;
    private float rotateAnglePerSecond;

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);

        lifeTime = bullet.info.lifeTime;
        timeCount = 0;
        rotateAnglePerSecond = bullet.info.rotateAnglePerSecond;
    
        //homingStartTime = bullet.info.homingStartTime;
        //homingEndTime = bullet.info.homingEndTime;
    }

    public override UpdateProperty Clone()
    {
        return new SpiralProperty();
    }

    public override void Update()
    {
        timeCount += Time.fixedDeltaTime;
        if (timeCount >= lifeTime)
        {
            delDestroyBullet();
        }
        bullet.RotateDirection(rotateAnglePerSecond * Time.fixedDeltaTime);
    }
}
