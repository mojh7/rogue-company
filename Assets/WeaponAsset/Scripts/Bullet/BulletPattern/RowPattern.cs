using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// 일렬로 총알 생성 패턴
public class RowPattern : BulletPattern
{
    private RowPatternInfo info;
    private Vector3 perpendicularVector;
    private Vector3 angleVector;
    private float randomAngle;
    private float currentAngle;

    // 기존 정보를 참조하는 방식으로 변수 초기화
    public RowPattern(RowPatternInfo patternInfo, int executionCount, float delay, bool isFixedOwnerDir, bool isFixedOwnerPos, CharacterInfo.OwnerType ownerType)
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
        if (null != info.randomBulletInfoList && info.randomBulletInfoList.Length > 0)
        {
            for (int i = 0; i < info.randomBulletInfoList.Length; i++)
            {
                info.randomBulletInfoList[i].Init();
            }
        }
        else
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
        return new RowPattern(info, executionCount, delay, isFixedOwnerDir, isFixedOwnerPos, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    /// <summary> 패턴대로 총알 생성 </summary>
    public override void CreateBullet(float damageIncreaseRate)
    {
        ApplyWeaponBuff();

        for (int i = 0; i < info.rotatingCount; i++)
        {
            randomAngle = Random.Range(-info.randomAngle, info.randomAngle);
            currentAngle = -info.initAngle + info.deltaAngle * i + randomAngle * accuracyIncrement;
            angleVector = MathCalculator.VectorRotate(ownerDirVec(), currentAngle);
            // 해당 angled에서 반시계방향 수직 벡터
            perpendicularVector = MathCalculator.VectorRotate(angleVector, -90);
            for (int j = 0; j < info.bulletCount; j++)
            {
                if (PatternCallType.WEAPON == patternCallType)
                {
                    if (AttackType.RANGED == weapon.GetAttackType())
                    {
                        if (weapon.HasCostForAttack())
                            weapon.UseAmmo();
                        else break;
                    }
                }
                if (info.ignoreOwnerDir)
                {
                    dirDegree = 0;
                }
                else
                {
                    dirDegree = ownerDirDegree();
                }
                
                AutoSelectBulletInfo(info.bulletInfo, info.randomBulletInfoList);

                createdObj = ObjectPoolManager.Instance.CreateBullet();
                createdObj.GetComponent<Bullet>().Init(bulletInfo, ownerBuff, ownerType,
                    weapon.GetMuzzlePos() + GetadditionalPos(info.ignoreOwnerDir, info.addDirVecMagnitude, info.additionalVerticalPos) + perpendicularVector * (info.initPos - info.deltaPos * j),
                    ownerDirDegree() + currentAngle, transferBulletInfo, info.childBulletCommonProperty.timeForOriginalShape);
                CreateChildBullets(info.childBulletInfoList, info.childBulletCommonProperty);
            }
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