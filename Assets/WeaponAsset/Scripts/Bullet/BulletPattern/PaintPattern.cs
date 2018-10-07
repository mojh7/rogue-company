using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;


////TODO : PaintPattern 만들어야 됨.

///// 네모, 세모, 별, 화살표 모양 등등 다양한 모양의 그림 패턴
//public class PaintPattern : BulletPattern
//{
//    private PaintPatternInfo info;
//    private GameObject parentBulletObj;
//    private Transform parentBulletTransform;

//    public PaintPattern(PaintPatternInfo patternInfo, int executionCount, float delay, bool isFixedOwnerDir, bool isFixedOwnerPos, CharacterInfo.OwnerType ownerType)
//    {
//        info = patternInfo;
//        this.executionCount = executionCount;
//        this.delay = delay;
//        this.isFixedOwnerDir = isFixedOwnerDir;
//        this.isFixedOwnerPos = isFixedOwnerPos;
//        this.ownerType = ownerType;
//    }
//    public override BulletPattern Clone()
//    {
//        return new PaintPattern(info, executionCount, delay, isFixedOwnerDir, isFixedOwnerPos, ownerType);
//    }

//    // 중심이 될 부모 총알과 부모 총알을 중심으로 배치될 자식 총알들 생성
//    public override void CreateBullet(float damageIncreaseRate)
//    {
//        /*
//        parentBulletObj = ObjectPoolManager.Instance.CreateBullet();
//        parentBulletObj.GetComponent<Bullet>().Init(info.bulletInfo.Clone(), ownerBuff, ownerType,
//                weapon.GetMuzzlePos() + GetadditionalPos(info.ignoreOwnerDir, info.addDirVecMagnitude, info.additionalVerticalPos),
//                dirDegree - info.initAngle + info.deltaAngle * i + additionalAngle + Random.Range(-info.randomAngle, info.randomAngle) * accuracyIncrement, transferBulletInfo);
//    */
//    }

//    public override void Init(Weapon weapon)
//    {
//        base.Init(weapon);
//        // 부모와 자식 총알 초기화
//        info.bulletInfo.Init();
//        for(int i = 0; i < info.childBulletInfoList.Count; i++)
//        {
//            info.childBulletInfoList[i].bulletInfo.Init();
//        }
//    }

//    public override void Init(BuffManager ownerBuff, TransferBulletInfo transferBulletInfo, DelGetDirDegree dirDegree,
//        DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
//    {
//        info.bulletInfo.Init();
//        base.Init(ownerBuff, transferBulletInfo, dirDegree, dirVec, pos, addDirVecMagnitude);
//    }

//    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
//    {
//        this.ownerType = ownerType;
//        CreateBullet(damageIncreaseRate);
//    }
    
//    public override void StopAttack()
//    {
//    }

//    public override void IncreaseAdditionalAngle()
//    {
//        additionalAngle += info.rotatedAnglePerExecution;
//    }
//}
