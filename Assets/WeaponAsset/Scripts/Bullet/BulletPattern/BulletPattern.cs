using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// 패턴 추가시 InfoGroup 에서 해당 pattern info도 만들고
// BulletPatternInfo 클래스에서 CreatePatternInfo함수에 내용 추가 해야됨.

// bullet 정보중에서 weapon->pattern->bullet 순으로 전달 되야할 정보들 담는 그릇
[System.Serializable]
public class TransferBulletInfo
{
    public WeaponType weaponType;
    public float bulletMoveSpeed;
    public float range;
    public float damage;
    public float knockBack;
    public float criticalChance;
}

[System.Serializable]
public abstract class BulletPattern
{
    protected GameObject createdObj;
    protected Weapon weapon;
    protected int executionCount;               // 한 사이클에서의 실행 횟수
    protected float delay;                      // 사이클 내에서의 delay
    protected float addDirVecMagnitude;         // onwer 총구 방향으로 총알 위치 추가 적인 위치 조절 값
    protected float accuracyIncrease;

    protected OwnerType ownerType;
    protected DelGetDirDegree ownerDirDegree;
    protected DelGetPosition ownerDirVec;
    protected DelGetPosition ownerPos;

    protected BuffManager ownerBuff;
    protected TransferBulletInfo transferBulletInfo;
    public float GetDelay()
    {
        return delay;
    }
    public float GetExeuctionCount()
    {
        return executionCount;
    }

    public virtual void Init(Weapon weapon)
    {
        this.weapon = weapon;
        // Owner 정보 등록
        ownerType = weapon.GetOwnerType();
        ownerDirDegree = weapon.GetOwnerDirDegree();
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();

        addDirVecMagnitude = weapon.info.addDirVecMagnitude;

        transferBulletInfo = new TransferBulletInfo();
        UpdateTransferBulletInfo();
    }
    public virtual void Init(DelGetDirDegree dirDegree, DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0) { }
    public abstract BulletPattern Clone();
    public abstract void StartAttack(float damageIncreaseRate, OwnerType ownerType); // 공격 시도 시작
    public virtual void StopAttack() { }  // 공격 시도 시작 후 멈췄을 때
    public abstract void CreateBullet(float damageIncreaseRate);

    /// <summary>
    /// 값이 0이 아니면 => 값 전달하기, 값이 0이면 전달할 필요 X
    /// </summary>
    public void UpdateTransferBulletInfo()
    {
        transferBulletInfo.weaponType = weapon.info.weaponType;

        if (weapon.info.bulletMoveSpeed != 0)
        {
            transferBulletInfo.bulletMoveSpeed = weapon.info.bulletMoveSpeed;
        }
        else
        {
            transferBulletInfo.bulletMoveSpeed = 0;
        }
        if (weapon.info.range != 0)
        {
            transferBulletInfo.range = weapon.info.range;
        }
        else
        {
            transferBulletInfo.range = 0;
        }
        if (weapon.info.damage != 0)
        {
            transferBulletInfo.damage = weapon.info.damage;
        }
        else
        {
            transferBulletInfo.damage = 0;
        }
        if (weapon.info.knockBack != 0)
        {
            transferBulletInfo.knockBack = weapon.info.knockBack;
        }
        else
        {
            transferBulletInfo.knockBack = 0;
        }
        if (weapon.info.criticalChance != 0)
        {
            transferBulletInfo.criticalChance = weapon.info.criticalChance;
        }
        else
        {
            transferBulletInfo.criticalChance = 0;
        }
    }
    public virtual void ApplyWeaponBuff()
    {
        accuracyIncrease = ownerBuff.WeaponTargetEffectTotal.accuracyIncrease;
    }
}