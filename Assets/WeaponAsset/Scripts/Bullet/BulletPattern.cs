using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

/* Weapon에서 공격 함수를 실행 할 때 실질적으로 Bullet을 생성하고 뿌리는 class
 * [현재]
 * 1. 다 방향 패턴
 *  - 총알 N 개 총구 방향 쏘는 것
 *  - 샷건 같은 일정 범위의 부채꼴로 퍼지며 총 쏘기
 *  - 총알 마다 각도의 간격이 일정하여 사방으로 쏘기 (상하좌우 4방향 or 8방향 퍼져서 쏘기)
 * 2. 일렬 패턴
 * 3. 레이저 패턴
 *  - 
 * 
 * -------------------
 * [예정]
 * 1. 기존 패턴 방식으로 만들 수 없는 경우 새로운 패턴 생성 방식 추가
 * 
 * 
 * [미정]
 *  - 
 */

[System.Serializable]
public abstract class BulletPattern
{
    protected GameObject createdObj;
    protected Weapon weapon;
    protected int executionCount;               // 한 사이클에서의 실행 횟수
    protected float delay;                      // 사이클 내에서의 delay
    protected float addDirVecMagnitude;         // onwer 총구 방향으로 총알 위치 추가 적인 위치 조절 값

    protected OwnerType ownerType;
    protected DelGetDirDegree ownerDirDegree;
    protected DelGetPosition ownerDirVec;
    protected DelGetPosition ownerPos;

    protected BuffManager ownerBuff;
    public float GetDelay()
    {
        return delay;
    }
    public float GetExeuctionCount()
    {
        return executionCount;
    }
    
    public abstract void Init(Weapon weapon);
    public virtual void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0) { }
    public abstract BulletPattern Clone();
    public abstract void StartAttack(float damageIncreaseRate, OwnerType ownerType); // 공격 시도 시작
    public abstract void StopAttack();  // 공격 시도 시작 후 멈췄을 때
    public abstract void CreateBullet(float damageIncreaseRate);
}

//  1~N개의 총알(1종류)을 다양한 방향으로 일정한 각도 텀을 두고 발사하는 패턴
public class MultiDirPattern : BulletPattern
{
    private MultiDirPatternInfo info;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public MultiDirPattern(MultiDirPatternInfo patternInfo, int executionCount, float delay, OwnerType ownerType)
    {
        info = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.ownerType = ownerType;
    }

    public override void Init(Weapon weapon)
    {
        this.weapon = weapon;
        info.bulletInfo.Init();

        ownerType = weapon.GetOwnerType();

        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        // Owner 방향, 위치 함수
        ownerDirDegree = weapon.GetOwnerDirDegree();
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();

        // weapon 정보 덮어 쓰려 할 때
        if (weapon.info.bulletMoveSpeed != 0)
        {
            info.speed = weapon.info.bulletMoveSpeed;
        }
        if (weapon.info.range != 0)
        {
            info.range = weapon.info.range;
        }
        if (weapon.info.damage != 0)
        {
            info.damage = weapon.info.damage;
        }
        if (weapon.info.knockBack != 0)
        {
            info.knockBack = weapon.info.knockBack;
        }
        if (weapon.info.criticalRate != 0)
        {
            info.criticalRate = weapon.info.criticalRate;
        }
    }

    public override void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        info.bulletInfo.Init();
        ownerDirDegree = dirDegree;
        ownerDirVec = dirVec;
        ownerPos = pos;
        this.addDirVecMagnitude = addDirVecMagnitude;
    }

    public override BulletPattern Clone()
    {
        //Debug.Log(patternId + ", " + info.bulletId + ", " + info.bulletSpriteId + ", " + executionCount);
        return new MultiDirPattern(info, executionCount, delay, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    public override void StopAttack()
    {

    }

    // 이미 저장된 정보 이용.
    public override void CreateBullet(float damageIncreaseRate)
    {
        for (int i = 0; i < info.bulletCount; i++)
        {
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerType, ownerPos() + ownerDirVec() * addDirVecMagnitude, ownerDirDegree() - info.initAngle + info.deltaAngle * i + Random.Range(-info.randomAngle, info.randomAngle)
                , info.speed, info.range, info.damage, info.knockBack, info.criticalRate);
        }
    }
}

// 일렬로 총알 생성 패턴
public class RowPattern : BulletPattern
{
    private RowPatternInfo info;
    private Vector3 perpendicularVector;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public RowPattern(RowPatternInfo patternInfo, int executionCount, float delay, OwnerType ownerType)
    {
        info = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.ownerType = ownerType;
    }

    public override void Init(Weapon weapon)
    {
        this.weapon = weapon;
        info.bulletInfo.Init();

        ownerType = weapon.GetOwnerType();

        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        // Owner 방향, 위치 함수
        ownerDirDegree = weapon.GetOwnerDirDegree();
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();

        // weapon 정보 덮어 쓰려 할 때
        if (weapon.info.bulletMoveSpeed != 0)
        {
            info.speed = weapon.info.bulletMoveSpeed;
        }
        if (weapon.info.range != 0)
        {
            info.range = weapon.info.range;
        }
        if (weapon.info.damage != 0)
        {
            info.damage = weapon.info.damage;
        }
        if (weapon.info.knockBack != 0)
        {
            info.knockBack = weapon.info.knockBack;
        }
        if (weapon.info.criticalRate != 0)
        {
            info.criticalRate = weapon.info.criticalRate;
        }
    }

    public override void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        info.bulletInfo.Init();
        ownerDirDegree = dirDegree;
        ownerDirVec = dirVec;
        ownerPos = pos;
        this.addDirVecMagnitude = addDirVecMagnitude;

        // weapon 정보 덮어 쓰려 할 때
        if (weapon.info.bulletMoveSpeed != 0)
        {
            info.speed = weapon.info.bulletMoveSpeed;
        }
        if (weapon.info.range != 0)
        {
            info.range = weapon.info.range;
        }
        if (weapon.info.damage != 0)
        {
            info.damage = weapon.info.damage;
        }
        if (weapon.info.knockBack != 0)
        {
            info.knockBack = weapon.info.knockBack;
        }
        if (weapon.info.criticalRate != 0)
        {
            info.criticalRate = weapon.info.criticalRate;
        }
    }

    public override BulletPattern Clone()
    {
        return new RowPattern(info, executionCount, delay, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    public override void StopAttack()
    {

    }

    /// <summary> 패턴대로 총알 생성 </summary>
    public override void CreateBullet(float damageIncreaseRate)
    {
        // 수직 벡터
        perpendicularVector = MathCalculator.VectorRotate(ownerDirVec(), -90);
        for (int i = 0; i < info.bulletCount; i++)
        {
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerType, ownerPos() + ownerDirVec() * addDirVecMagnitude + perpendicularVector * (info.initPos - info.deltaPos * i), ownerDirDegree() + Random.Range(-info.randomAngle, info.randomAngle)
                , info.speed, info.range, info.damage, info.knockBack, info.criticalRate);
        }
    }
}



// 레이저 패턴
/* bullet 1개만 씀
 * 공격 시도를 하면(Player는 공격 버튼 다운) 1회에 한 해서만 laser bullet 생성
 * raycast, line renderer 그리기, 충돌 처리 모두 laser 관련 속성에서 실행됨
 * 레이저 패턴은 딱 공격 버튼 누르면 생성하고 공격 버튼 떼면 레이저 없애는 역할만 함.
 */
public class LaserPattern : BulletPattern
{
    private LaserPatternInfo info;
    private DelDestroyBullet destroyBullet;
    private bool canCreateLaser;
    

    // 기존에 저장된 정보 이외의 내용으로 변수 초기화
    public LaserPattern(LaserPatternInfo patternInfo, OwnerType ownerType)
    {
        this.info = patternInfo;
        this.executionCount = 1;
        this.delay = 0;
        this.ownerType = ownerType;
    }

    public override void Init(Weapon weapon)
    {
        this.weapon = weapon;
        info.bulletInfo.Init();
        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();
        canCreateLaser = true;

        // weapon 정보 덮어 쓰려 할 때
        if (weapon.info.damage != 0)
        {
            info.damage = weapon.info.damage;
        }
        if (weapon.info.knockBack != 0)
        {
            info.knockBack = weapon.info.knockBack;
        }
        if (weapon.info.criticalRate != 0)
        {
            info.criticalRate = weapon.info.criticalRate;
        }
    }

    public override BulletPattern Clone()
    {
        return new LaserPattern(info, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, OwnerType ownerType)
    {
        this.ownerType = ownerType;
        if (canCreateLaser == true)
        {
            CreateBullet(damageIncreaseRate);
            canCreateLaser = false;
        }
        
    }

    public override void StopAttack()
    {
        canCreateLaser = true;
        destroyBullet();
    }

    // 이미 저장된 정보 이용.
    public override void CreateBullet(float damageIncreaseRate)
    {
        createdObj = ObjectPoolManager.Instance.CreateBullet();
        createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerType, addDirVecMagnitude, ownerPos, ownerDirVec
            , info.damage, info.knockBack, info.criticalRate);
        destroyBullet = createdObj.GetComponent<Bullet>().DestroyBullet;
    }
}