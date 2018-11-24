using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

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
    private float timeForUseAmmo;

    private Bullet createdBullet;

    private const float periodOfUsingLaserAmmo = 0.1f;

    // 기존에 저장된 정보 이외의 내용으로 변수 초기화
    public LaserPattern(LaserPatternInfo patternInfo, CharacterInfo.OwnerType ownerType)
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
        canCreateLaser = true;

        timeForUseAmmo = 0;
    }

    public override BulletPattern Clone()
    {
        return new LaserPattern(info, ownerType);
    }

    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
    {
        this.ownerType = ownerType;
        if(timeForUseAmmo > periodOfUsingLaserAmmo)
        {
            timeForUseAmmo -= periodOfUsingLaserAmmo;
            weapon.UseAmmo();
        }
        if (false == weapon.HasCostForAttack())
        {
            StopAttack();
            return;
        }

        if (canCreateLaser == true)
        {
            canCreateLaser = false;
            CreateBullet(damageIncreaseRate);
        }
        timeForUseAmmo += Time.deltaTime;
        //Debug.Log(timeForUseAmmo + ", " + Time.time + ", " + Time.deltaTime +", " + Time.realtimeSinceStartup);
    }

    public override void StopAttack()
    {
        canCreateLaser = true;
        destroyBullet();
    }

    // 이미 저장된 정보 이용.
    public override void CreateBullet(float damageIncreaseRate)
    {
        AutoSelectBulletInfo(info.bulletInfo, info._pRoot);

        createdObj = ObjectPoolManager.Instance.CreateBullet();
        createdBullet = createdObj.GetComponent<Bullet>();
        createdBullet.Init(bulletInfo, ownerBuff, ownerType, addDirVecMagnitude, additionalVerticalPos,
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
}
