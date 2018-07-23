using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

//  1~N개의 총알(1종류)을 다양한 방향으로 일정한 각도 텀을 두고 발사하는 패턴
public class MultiDirPattern : BulletPattern
{
    private MultiDirPatternInfo info;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public MultiDirPattern(MultiDirPatternInfo patternInfo, int executionCount, float delay, CharacterInfo.OwnerType ownerType)
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

    public override void Init(BuffManager ownerBuff, TransferBulletInfo transferBulletInfo, DelGetDirDegree dirDegree,
        DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        info.bulletInfo.Init();
        base.Init(ownerBuff, transferBulletInfo, dirDegree, dirVec, pos, addDirVecMagnitude);
    }

    public override BulletPattern Clone()
    {
        return new MultiDirPattern(info, executionCount, delay, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    // 이미 저장된 정보 이용.
    public override void CreateBullet(float damageIncreaseRate)
    {
        ApplyWeaponBuff();
        for (int i = 0; i < info.bulletCount; i++)
        {
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerBuff, ownerType, ownerPos() + ownerDirVec() * addDirVecMagnitude,
                ownerDirDegree() - info.initAngle + info.deltaAngle * i + Random.Range(-info.randomAngle, info.randomAngle) * accuracyIncrement, transferBulletInfo);
        }
    }

    public override void ApplyWeaponBuff()
    {
        base.ApplyWeaponBuff();
    }
}
