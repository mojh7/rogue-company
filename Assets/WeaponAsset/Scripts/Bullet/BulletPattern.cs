using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletData;
using DelegateCollection;

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
    protected int patternId;        // 패턴 내에서의 id
    protected int executionCount;   // 한 사이클에서의 실행 횟수
    protected float delay;          // 사이클 내에서의 delay
    protected float addDirVecMagnitude;         // onwer 총구 방향으로 총알 위치 추가 적인 위치 조절 값
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
    public abstract void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0);
    public abstract BulletPattern Clone();
    public abstract void StartAttack(float damageIncreaseRate); // 공격 시도 시작
    public abstract void StopAttack();  // 공격 시도 시작 후 멈췄을 때
    public abstract void CreateBullet(float damageIncreaseRate);
}

//  1~N개의 총알(1종류)을 다양한 방향으로 일정한 각도 텀을 두고 발사하는 패턴
public class MultiDirPattern : BulletPattern
{
    private MultiDirPatternInfo info;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public MultiDirPattern(int patternId, int executionCount, float delay)
    {
        this.patternId = patternId;
        info = DataStore.Instance.GetMultiDirPatternInfo(patternId);
        this.executionCount = executionCount;
        this.delay = delay;
    }

    public override void Init(Weapon weapon)
    {
        this.weapon = weapon;
        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        // Owner 방향, 위치 함수
        ownerDirDegree = weapon.GetownerDirDegree();
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();
        if(weapon.info.bulletMoveSpeed != 0)
        {
            info.speed = weapon.info.bulletMoveSpeed;
        }
        if (weapon.info.range != 0)
        {
            info.range = weapon.info.range;
        }
    }

    public override void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        ownerDirDegree = dirDegree;
        ownerDirVec = dirVec;
        ownerPos = pos;
        this.addDirVecMagnitude = addDirVecMagnitude;
    }

    public override BulletPattern Clone()
    {
        //Debug.Log(patternId + ", " + info.bulletId + ", " + info.bulletSpriteId + ", " + executionCount);
        return new MultiDirPattern(patternId, executionCount, delay);
    }

    public override void StartAttack(float damageIncreaseRate)
    {
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
            createdObj.GetComponent<Bullet>().Init(info.bulletId, info.bulletAnimationName, info.speed, info.range, info.effectId,  ownerPos() + ownerDirVec() * addDirVecMagnitude, ownerDirDegree() - info.initAngle + info.deltaAngle * i + Random.Range(-info.randomAngle, info.randomAngle));
        }
    }
}

// 일렬로 총알 생성 패턴
public class RowPattern : BulletPattern
{
    private RowPatternInfo info;
    private Vector3 perpendicularVector;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public RowPattern(int patternId, int executionCount, float delay)
    {
        this.patternId = patternId;
        info = DataStore.Instance.GetRowPatternInfo(patternId);
        this.executionCount = executionCount;
        this.delay = delay;
    }

    public override void Init(Weapon weapon)
    {
        this.weapon = weapon;
        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        // Owner 방향, 위치 함수
        ownerDirDegree = weapon.GetownerDirDegree();
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();

        // weapon에서 값 가져올 필요가 있는 경우 가져옴.
        if (weapon.info.bulletMoveSpeed != 0)
        {
            info.speed = weapon.info.bulletMoveSpeed;
        }
        if (weapon.info.range != 0)
        {
            info.range = weapon.info.range;
        }
    }

    public override void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        ownerDirDegree = dirDegree;
        ownerDirVec = dirVec;
        ownerPos = pos;
        this.addDirVecMagnitude = addDirVecMagnitude;
    }

    public override BulletPattern Clone()
    {
        return new RowPattern(patternId, executionCount, delay);
    }

    public override void StartAttack(float damageIncreaseRate)
    {
        CreateBullet(damageIncreaseRate);
    }

    public override void StopAttack()
    {

    }

    // 이미 저장된 정보 이용.
    public override void CreateBullet(float damageIncreaseRate)
    {
        perpendicularVector = MathCalculator.VectorRotate(ownerDirVec(), -90);
        for (int i = 0; i < info.bulletCount; i++)
        {
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletId, info.bulletAnimationName, info.speed, info.range, info.effectId, ownerPos() + ownerDirVec() * addDirVecMagnitude + perpendicularVector * info.initPos -perpendicularVector * info.deltaPos * i, ownerDirDegree() + Random.Range(-info.randomAngle, info.randomAngle));
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
    public LaserPattern(int patternId)
    {
        this.patternId = patternId;
        info = DataStore.Instance.GetLaserPatternInfo(patternId);
        this.executionCount = 1;
        this.delay = 0;
    }

    public override void Init(Weapon weapon)
    {
        this.weapon = weapon;
        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();
        canCreateLaser = true;
    }
    public override void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
    }


    public override BulletPattern Clone()
    {
        return new LaserPattern(patternId);
    }

    public override void StartAttack(float damageIncreaseRate)
    {
        if(canCreateLaser == true)
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
        createdObj.GetComponent<Bullet>().Init(info.bulletId, addDirVecMagnitude, ownerPos, ownerDirVec);
        destroyBullet = createdObj.GetComponent<Bullet>().DestroyBullet;
    }
}