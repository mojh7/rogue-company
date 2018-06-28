using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

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
        base.Init(weapon);
        info.bulletInfo.Init();
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
        return new RowPattern(info, executionCount, delay, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    /// <summary> 패턴대로 총알 생성 </summary>
    public override void CreateBullet(float damageIncreaseRate)
    {
        // 수직 벡터
        perpendicularVector = MathCalculator.VectorRotate(ownerDirVec(), -90);
        for (int i = 0; i < info.bulletCount; i++)
        {
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerBuff, ownerType, ownerPos() + ownerDirVec() * addDirVecMagnitude + perpendicularVector * (info.initPos - info.deltaPos * i),
                ownerDirDegree() + Random.Range(-info.randomAngle, info.randomAngle) * accuracyIncrease, transferBulletInfo);
        }
    }

    public override void ApplyWeaponBuff()
    {
        base.ApplyWeaponBuff();
    }
}