using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// 고정된 owner 패턴
/* 
 * 몬스터 화염 공격, 돌진 공격등 laserPattern 처럼 한 번만 생성해서
 * 계속 owner 위치와 각도에 따라서 bullet 위치, 각도 업데이트 해줘야 하기 위해서 만든 패턴
 */
public class FixedOwnerPattern : BulletPattern
{
    private FixedOwnerPatternInfo info;
    private DelDestroyBullet destroyBullet;
    private bool canCreateBullet;

    private Bullet createdBullet;

    // 기존에 저장된 정보 이외의 내용으로 변수 초기화
    public FixedOwnerPattern(FixedOwnerPatternInfo patternInfo, CharacterInfo.OwnerType ownerType)
    {
        this.info = patternInfo;
        this.executionCount = 1;
        this.delay = 0;
        this.ownerType = ownerType;
    }

    public override void Init(Weapon weapon)
    {
        base.Init(weapon);
        info.bulletInfo.Init();
        canCreateBullet = true;
    }

    public override BulletPattern Clone()
    {
        return new FixedOwnerPattern(info, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
    {
        this.ownerType = ownerType;
        if (canCreateBullet == true)
        {
            CreateBullet(damageIncreaseRate);
            canCreateBullet = false;
        }
    }

    public override void StopAttack()
    {
        canCreateBullet = true;
        destroyBullet();
    }

    // 이미 저장된 정보 이용.
    public override void CreateBullet(float damageIncreaseRate)
    {
        createdObj = ObjectPoolManager.Instance.CreateBullet();
        createdBullet = createdObj.GetComponent<Bullet>();
        createdBullet.Init(info.bulletInfo.Clone(), ownerBuff, ownerType, addDirVecMagnitude,
            weapon.GetMuzzlePos() + GetadditionalPos(),
            ownerPos, ownerDirVec, ownerDirDegree, transferBulletInfo);

        destroyBullet = createdObj.GetComponent<Bullet>().DestroyBullet;
    }

    public override void ApplyWeaponBuff()
    {
        base.ApplyWeaponBuff();
    }

    public override void IncreaseAdditionalAngle()
    {
        additionalAngle += info.rotatedAnglePerExecution;
    }

    protected override Vector3 GetadditionalPos()
    {
        return ownerDirVec() * info.addDirVecMagnitude + GetVerticalVector() * info.additionalVerticalPos;
    }
}
