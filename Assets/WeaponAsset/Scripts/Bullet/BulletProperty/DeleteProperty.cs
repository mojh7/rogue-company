using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* DeleteProperty class
 * 총알이 삭제(회수) 될 때에 관련된 클래스
 * [현재]
 * 1. BaseDeleteProperty
 *  - 일반적인 삭제 속성, 현재는 오브젝트 풀에서 bullet 오브젝트를 회수하는 일만 함.
 *  
 * 2. LaserDeleteProperty
 *  - 레이저 전용 삭제 속성, 원래 레이저 총알이면 따로 그때마다 라인 렌더러 컴포넌트 붙인다거나 추가 처리를 할 것 같아서
 *  - 그것에 따른 소멸자 개념으로 처리하려고 따로 빼놓았는데 아마 구현 예정상 bullet 안에다가 라인 렌더러, 트레일 렌더러, 각종 컬라이더 다 넣어놓고
 *  - 총알 종류에 따라서 enable만 true or false 해서 관리 할 것 같아서 레이저 삭제 속성이 현재 따로 하는 일은 없음
 *  
 * 3. DeleteAfterSummonBulletProperty
 *  - 본래 총알이 삭제 될 때 새로운 bullet이 똑같은 position에 생성됨
 *  - 수류탄, 로켓런쳐 같은 본래 bullet이 터지고 폭발하는 총알에 쓰일 delete 속성
 *  
 * 4. DeleteAfterSummonPatternProperty
 *  - 총알 삭제시 bulletPattern 생성 후 삭제
 *  - 현재는 multiPattern만 1회 생성으로 되어있고 추후 필요 시
 *  - SummonUpdate 속성 처럼 다양한 패턴과 횟수의 pattern을 생성할 수도 있음.
 * -------------------
 * [예정]
 * 1. 
 * 
 * [미정]
 * 1. 
 */

public abstract class DeleteProperty : BulletProperty
{
    public abstract DeleteProperty Clone();
    public abstract void DestroyBullet();
}

/// <summary> 기본 총알 삭제, effect만 생성 </summary>
public class BaseDeleteProperty : DeleteProperty
{
    public override DeleteProperty Clone()
    {
        return new BaseDeleteProperty();
    }

    public override void DestroyBullet()
    {
        // ObjectPoolManager.Instance.CreateEffect(bullet.info.effectId, bulletTransform.position);
        TestScript.Instance.CreateEffect(bullet.info.effectId, bulletTransform.position);
        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
    }
}

/// <summary> 레이저 전용 삭제 속성 </summary>
public class LaserDeleteProperty : DeleteProperty
{
    //private LineRenderer lineRenderer;

    public override DeleteProperty Clone()
    {
        return new LaserDeleteProperty();
    }

    public override void DestroyBullet()
    {

        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        //lineRenderer = bullet.GetLineRenderer();
    }
}

/// <summary> 총알 삭제시 position그대로 새로운 bullet 생성 </summary>
public class DeleteAfterSummonBulletProperty : DeleteProperty
{
    private GameObject createdObj;

    public override DeleteProperty Clone()
    {
        return new DeleteAfterSummonBulletProperty();
    }

    public override void DestroyBullet()
    {
        createdObj = ObjectPoolManager.Instance.CreateBullet();
        createdObj.GetComponent<Bullet>().Init(bullet.info.deleteAfterSummonBulletInfo.Clone(), ownerBuff, transferBulletInfo, bullet.GetOwnerType(), bulletTransform.position);
        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        bullet.info.deleteAfterSummonBulletInfo.Init();
    }
}

// 이거 아직 제대로 구현 안함
// 손봐야 될게 있음

/// <summary> 총알 삭제시 새로운 BulletPattern 생성</summary>
public class DeleteAfterSummonPatternProperty : DeleteProperty
{
    private BulletPattern summonBulletPattern;


    public override DeleteProperty Clone()
    {
        return new DeleteAfterSummonPatternProperty();
    }

    public override void DestroyBullet()
    {
        summonBulletPattern.Init(bullet.GetOwnerBuff(), bullet.GetTransferBulletInfo(), () => 0f, () => Vector3.zero, ()=>bulletTransform.transform.position);
        summonBulletPattern.StartAttack(1.0f, bullet.GetOwnerType());
        ObjectPoolManager.Instance.DeleteBullet(bulletObj);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        summonBulletPattern = new MultiDirPattern(bullet.info.deleteAfterSummonPatternInfo as MultiDirPatternInfo, 1, 0, bullet.GetOwnerType());
    }
}
