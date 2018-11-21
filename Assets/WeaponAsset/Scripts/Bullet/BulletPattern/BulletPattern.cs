using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

// 패턴 추가시 InfoGroup 에서 해당 pattern info도 만들고
// BulletPatternInfo 클래스에서 CreatePatternInfo함수에 내용 추가 해야됨.

public enum PatternCallType { WEAPON, BULLET }

// bullet 정보중에서 weapon->pattern->bullet 순으로 전달 되야할 정보들
[System.Serializable]
public class TransferBulletInfo
{
    public WeaponType weaponType;
    public float bulletMoveSpeed;
    public float range;
    public float damage;
    public float criticalChance;
    public float chargedDamageIncrement;

    public TransferBulletInfo()
    {
    }

    public TransferBulletInfo(TransferBulletInfo info)
    {
        weaponType = info.weaponType;
        bulletMoveSpeed = info.bulletMoveSpeed;
        range = info.range;
        damage = info.damage;
        criticalChance = info.criticalChance;
        chargedDamageIncrement = info.chargedDamageIncrement;
    }
}

[System.Serializable]
public abstract class BulletPattern
{
    protected GameObject createdObj;
    protected Transform parentBulletTransform;
    protected GameObject childBulletObj;
    protected Weapon weapon;
    protected int executionCount;               // 한 사이클에서의 실행 횟수
    protected float delay;                      // 사이클 내에서의 delay
    protected bool isFixedOwnerDir;
    protected bool isFixedOwnerPos;
    protected float addDirVecMagnitude;         // onwer 바라보는 방향 추가적인 수평 위치
    protected float additionalVerticalPos;      // onwer 바라보는 방향 추가적인 수직 위치
    protected float accuracyIncrement;

    protected float additionalAngle;
    protected CharacterInfo.OwnerType ownerType;
    protected DelGetDirDegree ownerDirDegree;
    protected DelGetPosition ownerDirVec;
    protected DelGetPosition ownerPos;
    protected float dirDegree;

    protected BuffManager ownerBuff;
    protected TransferBulletInfo transferBulletInfo;

    protected WeaponTargetEffect totalInfo;
    protected WeaponTargetEffect effectInfo;

    protected Vector3 muzzlePos;

    protected PatternCallType patternCallType;
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
        patternCallType = PatternCallType.WEAPON;
        this.weapon = weapon;
        // Owner 정보 등록
        ownerType = weapon.GetOwnerType();
        ownerDirDegree = weapon.GetOwnerDirDegree();
        ownerDirVec = weapon.GetOwnerDirVec();
        ownerPos = weapon.GetOwnerPos();
        ownerBuff = weapon.GetOwnerBuff();

        addDirVecMagnitude = weapon.info.addDirVecMagnitude;
        additionalVerticalPos = weapon.info.additionalVerticalPos;

        transferBulletInfo = new TransferBulletInfo();
        UpdateTransferBulletInfo();

        totalInfo = ownerBuff.WeaponTargetEffectTotal[0];
        effectInfo = ownerBuff.WeaponTargetEffectTotal[(int)transferBulletInfo.weaponType];
    }
    /// <summary> updateProperty - SummonProperty용 초기화 </summary>
    public virtual void Init(BuffManager ownerBuff, TransferBulletInfo transferBulletInfo, DelGetDirDegree dirDegree,
        DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        patternCallType = PatternCallType.BULLET;
        ownerDirDegree = dirDegree;
        ownerDirVec = dirVec;
        ownerPos = pos;
        this.ownerBuff = ownerBuff;
        this.addDirVecMagnitude = addDirVecMagnitude;
        this.transferBulletInfo = new TransferBulletInfo
        {
            weaponType = transferBulletInfo.weaponType
        };

        totalInfo = ownerBuff.WeaponTargetEffectTotal[0];
        effectInfo = ownerBuff.WeaponTargetEffectTotal[(int)transferBulletInfo.weaponType];
    }
    public abstract BulletPattern Clone();
    public abstract void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType); // 공격 시도 시작
    public virtual void StopAttack() { }  // 공격 시도 시작 후 멈췄을 때
    public abstract void CreateBullet(float damageIncreaseRate);

    /// <summary>
    /// 값이 0이 아니면 => 값 전달하기, 값이 0이면 전달할 필요 X
    /// </summary>
    protected void UpdateTransferBulletInfo()
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
        accuracyIncrement = totalInfo.accuracyIncrement * effectInfo.accuracyIncrement;
    }

    public virtual void InitAdditionalAngle()
    {
        additionalAngle = 0;
    }
    public abstract void IncreaseAdditionalAngle();

    protected Vector3 GetVerticalVector()
    {
        if (-90 <= ownerDirDegree() && ownerDirDegree() < 90)
        {
            return MathCalculator.VectorRotate(ownerDirVec(), 90);
        }
        else
        {
            return MathCalculator.VectorRotate(ownerDirVec(), -90);
        }
    }

    protected Vector3 GetadditionalPos(bool ignoreOwnerDir, float addDirVecMagnitude, float additionalVerticalPos)
    {
        Vector3 verticalVector;
        if (ignoreOwnerDir)
        {
            verticalVector = Vector3.up;
        }
        else
        {
            verticalVector = GetVerticalVector();
        }
        return ownerDirVec() * addDirVecMagnitude + verticalVector * additionalVerticalPos;
    }

    protected virtual void CreateChildBullets(List<ChildBulletInfo> childBulletInfoList, ChildBulletCommonProperty childBulletCommonProperty)
    {
        parentBulletTransform = createdObj.GetComponent<Transform>();
        for (int i = 0; i < childBulletInfoList.Count; i++)
        {
            for (int j = 0; j < childBulletInfoList[i].initVectorList.Count; j++)
            {
                childBulletObj = ObjectPoolManager.Instance.CreateBullet();
                childBulletObj.GetComponent<Bullet>().Init(childBulletInfoList[i].bulletInfo.Clone(), ownerBuff, ownerType, parentBulletTransform,
                    childBulletCommonProperty, transferBulletInfo, childBulletInfoList[i].initVectorList[j]);
            }

            InitVector initVector = new InitVector();
            Vector3 initPos = Vector3.zero;
            for (int j = 0; j < childBulletInfoList[i].initPosList.Count; j++)
            {
                initPos = new Vector3(childBulletInfoList[i].initPosList[j].x, childBulletInfoList[i].initPosList[j].y, 0);
                initVector.magnitude = initPos.magnitude;
                initVector.dirDegree = MathCalculator.GetDegFromVector(initPos);
                childBulletObj = ObjectPoolManager.Instance.CreateBullet();
                childBulletObj.GetComponent<Bullet>().Init(childBulletInfoList[i].bulletInfo.Clone(), ownerBuff, ownerType, parentBulletTransform,
                    childBulletCommonProperty, transferBulletInfo, initVector);
            }

            Vector3 childBulletPos = Vector3.zero;
            Vector3 deltaVector = Vector3.zero;
            for (int j = 0; j < childBulletInfoList[i].lineInfoList.Count; j++)
            {
                childBulletPos = new Vector3(childBulletInfoList[i].lineInfoList[j].initPos.x, childBulletInfoList[i].lineInfoList[j].initPos.y, 0);
                if (childBulletInfoList[i].lineInfoList[j].childBulletCount > 1)
                    deltaVector = (childBulletInfoList[i].lineInfoList[j].magnitude) / (childBulletInfoList[i].lineInfoList[j].childBulletCount - 1) *
                        MathCalculator.VectorRotate(Vector3.right, childBulletInfoList[i].lineInfoList[j].dirDegree);
                else
                    deltaVector = Vector3.zero;
                for (int k = 0; k < childBulletInfoList[i].lineInfoList[j].childBulletCount; k++)
                {
                    // if(A and B) or (C and D)
                    // else-> if (!A or !B) And (!C or !D)
                    // k = 0 일 때 시작 점 안 그리고, k = last 일 때 끝 점 안 그리면 패스
                    // k = 0 = last 일 때는 canDrawStart, End 모두 true여야함.
                    if ( (0 != k || childBulletInfoList[i].lineInfoList[j].canDrawStartPoint)
                        && (childBulletInfoList[i].lineInfoList[j].childBulletCount - 1 != k || childBulletInfoList[i].lineInfoList[j].canDrawEndPoint))
                    {
                        initVector.magnitude = initPos.magnitude;
                        initVector.dirDegree = MathCalculator.GetDegFromVector(initPos);
                        childBulletObj.GetComponent<Bullet>().Init(childBulletInfoList[i].bulletInfo.Clone(), ownerBuff, ownerType, parentBulletTransform,
                        childBulletCommonProperty, transferBulletInfo, initVector);
                    }
                    childBulletPos += deltaVector;
                }
            }
        }
    }
}