using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

//TODO : PaintPattern 만들어야 됨.

/// 네모, 세모, 별, 화살표 모양 등등 다양한 모양의 그림 패턴
public class PaintPattern : BulletPattern
{
    private SpreadPatternInfo info;
    private float sectorAngle;
    private int bulletCount;

    public PaintPattern(SpreadPatternInfo patternInfo, int executionCount, float delay, bool isFixedOwnerDir, bool isFixedOwnerPos, CharacterInfo.OwnerType ownerType)
    {
        info = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.isFixedOwnerDir = isFixedOwnerDir;
        this.isFixedOwnerPos = isFixedOwnerPos;
        this.ownerType = ownerType;
    }
    public override BulletPattern Clone()
    {
        return new PaintPattern(info, executionCount, delay, isFixedOwnerDir, isFixedOwnerPos, ownerType);
    }

    public override void CreateBullet(float damageIncreaseRate)
    {

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

    public override void IncreaseAdditionalAngle()
    {
        additionalAngle += info.rotatedAnglePerExecution;
    }
}