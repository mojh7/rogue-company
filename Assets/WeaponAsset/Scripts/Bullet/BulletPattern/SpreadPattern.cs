using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// 일정 부채꼴 범위로 퍼져나가는 총알로 따로 각도 계산 없이 퍼질 부채꼴 범위, 총알 갯수만 있으면 되고
// 패시브 아이템 중 샷건류 퍼져나가는 총알 수 늘리는거 적용 하려고 따로 만듬(multi pattern 으로 해도 되는데 그냥 따로 만듬)
public class SpreadPattern : BulletPattern
{
    private SpreadPatternInfo info;
    private float sectorAngle;
    private int bulletCount;
    public SpreadPattern(SpreadPatternInfo patternInfo, int executionCount, float delay, CharacterInfo.OwnerType ownerType)
    {
        info = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.ownerType = ownerType;
    }
    public override BulletPattern Clone()
    {
        return new SpreadPattern(info, executionCount, delay, ownerType);
    }

    public override void CreateBullet(float damageIncreaseRate)
    {
        ApplyWeaponBuff();
        float initAngle = sectorAngle / 2;
        float deltaAngle = sectorAngle / (bulletCount - 1);
        //Debug.Log(bulletCount);
        for (int i = 0; i < bulletCount; i++)
        {
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerBuff, ownerType, ownerPos() + ownerDirVec() * addDirVecMagnitude,
                ownerDirDegree() - initAngle + deltaAngle * i + Random.Range(-info.randomAngle, info.randomAngle) * accuracyIncrease, transferBulletInfo);
        }
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

    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    public override void StopAttack()
    {
    }

    public override void ApplyWeaponBuff()
    {
        base.ApplyWeaponBuff();
        // 정확도
        accuracyIncrease *= ownerBuff.WeaponTargetEffectTotal.shotgunsAccuracyIncrease;
        sectorAngle = info.sectorAngle * accuracyIncrease;
        // 샷건 발사 수 증가
        bulletCount = info.bulletCount + ownerBuff.WeaponTargetEffectTotal.shotgunBulletCountIncrease;
    }
}