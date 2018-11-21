using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using CharacterInfo;

//  1~N개의 총알(1종류)을 다양한 방향으로 일정한 각도 텀을 두고 발사하는 패턴
public class MultiDirPattern : BulletPattern
{
    private MultiDirPatternInfo info;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public MultiDirPattern(MultiDirPatternInfo patternInfo, int executionCount, float delay, bool isFixedOwnerDir, bool isFixedOwnerPos, CharacterInfo.OwnerType ownerType)
    {
        info = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.isFixedOwnerDir = isFixedOwnerDir;
        this.isFixedOwnerPos = isFixedOwnerPos;
        this.ownerType = ownerType;
    }

    public override void Init(Weapon weapon)
    {
        base.Init(weapon);
        info.bulletInfo.Init();
        for (int i = 0; i < info.childBulletInfoList.Count; i++)
        {
            info.childBulletInfoList[i].bulletInfo.Init();
        }
    }

    public override void Init(BuffManager ownerBuff, TransferBulletInfo transferBulletInfo, DelGetDirDegree dirDegree,
        DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        info.bulletInfo.Init();
        base.Init(ownerBuff, transferBulletInfo, dirDegree, dirVec, pos, addDirVecMagnitude);
    }

    public override BulletPattern Clone()
    {
        return new MultiDirPattern(info, executionCount, delay, isFixedOwnerDir, isFixedOwnerPos, ownerType);
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
            if(PatternCallType.WEAPON == patternCallType)
            {
                if(AttackType.RANGED == weapon.GetAttackType())
                {
                    if (weapon.HasCostForAttack())
                        weapon.UseAmmo();
                    else break;
                }
                muzzlePos = weapon.GetMuzzlePos();
            }
            else
            {
                muzzlePos = ownerPos() + ownerDirVec() * info.addDirVecMagnitude + MathCalculator.VectorRotate(ownerDirVec(), 90) * info.additionalVerticalPos;
            }
            if (info.ignoreOwnerDir)
            {
                dirDegree = 0;
            }
            else
            {
                dirDegree = ownerDirDegree();
            }
            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerBuff, ownerType,
                muzzlePos + GetadditionalPos(info.ignoreOwnerDir, info.addDirVecMagnitude, info.additionalVerticalPos),
                dirDegree - info.initAngle + info.deltaAngle * i + additionalAngle + Random.Range(-info.randomAngle, info.randomAngle) * accuracyIncrement,
                transferBulletInfo, info.childBulletCommonProperty.timeForOriginalShape); 
            CreateChildBullets(info.childBulletInfoList, info.childBulletCommonProperty);
        }
    }

    public override void ApplyWeaponBuff()
    {
        base.ApplyWeaponBuff();
    }

    public override void IncreaseAdditionalAngle()
    {
        additionalAngle += info.rotatedAnglePerExecution;
    }  
}
