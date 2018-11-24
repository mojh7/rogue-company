using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// parentBullet 좌표를 원점으로 멀어지는 길이(크기)와 방향(Vector 비슷)
/// </summary>
[System.Serializable]
public struct InitVector
{
    public float magnitude;
    public float dirDegree;
}

[System.Serializable]
public struct LineInfo
{
    public Vector2 initPos;
    public float magnitude;
    public float dirDegree;
    public int childBulletCount;
    [Tooltip("시작 점 그릴 수 있는지")]
    public bool canDrawStartPoint;
    [Tooltip("끝 점 그릴 수 있는지")]
    public bool canDrawEndPoint;
}

[System.Serializable]
public struct CircleShapeInfo
{
    public Vector2 centerOfCircle;
    public int childBulletCount;
    [Header("초기 bullet 놓일 각도, 시계방향 : +")]
    public float initAngle;
    [Tooltip("반 지름")]
    public float radius;
}

/// <summary>
/// ChildBullet에서 bulletInfo랑 벡터 값(원점에서 멀어지는 길이와 방향)
/// </summary>
[System.Serializable]
public struct ChildBulletInfo
{
    public BulletInfo bulletInfo;
    [Header("Chile Bullet 원점에서의 길이와 방향으로 좌표 설정할 때 값")]
    public List<InitVector> initVectorList;
    [Header("Chile Bullet 원점에서의 x, y 좌표로 설정할 때 값")]
    public List<Vector2> initPosList;
    public List<LineInfo> lineInfoList;
    public List<CircleShapeInfo> circleShapeInfoList;
}

[System.Serializable]
public struct ChildBulletCommonProperty
{
    [Header("parent Bullet 보다 작거나 같은 값으로 공통된 lifeTime")]
    public float childBulletLifeTime;
    [Header("원점(parentBullet position)에서 원래 모양으로 만들어 질 때 필요한 시간")]
    public float timeForOriginalShape;
    [Header("원점(parentBullet position) 기준으로 회전하는 각도(+ : 반시계 방향, - : 시계 방향")]
    public float rotatedAngleForChild;
    [Header("원점(parentBullet position)에서 멀어지는 속도 (+ : 멀어짐, - : 가까워 짐)")]
    public float movingAwaySpeed;
}

// TODO: pattern clone 해주는 함수 필요 없으나 필요하다고 생각 하면 넣을 예정.

public class BulletPatternInfo : ScriptableObject
{
    [Tooltip("적용하거나 쓰이는 곳, 사용하는 사람, 간단한 설명 등등 이것 저것 메모할 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    // weapon Info에서 다운 캐스팅 할 때 pattern type 구분용
    // public BulletPatternType patternType;

    [Header("총알 발사 할 때 BulletInfo 한 개만 사용하고 싶을 때 사용")]
    [Tooltip("총알 Info, laser 무기는 꼭 laserUpdate와 laserDelete 속성을 가진 총알 만 쓰기")]
    public BulletInfo bulletInfo;

    [Header("여러 개의 총알 중 랜덤하게 bulletInfo 사용하고 싶을 때 사용")]
    public BulletInfo[] randomBulletInfoList;

    [Header("BulletPattern 공통 속성")]
    public float addDirVecMagnitude;
    public float additionalVerticalPos;
    public float rotatedAnglePerExecution;
    public bool ignoreOwnerDir;

    /// <summary> bulletPatternInfo 클래스를 알맞은 클래스로 다운 캐스팅하고 bulletPattern을 생성하여 반환한다 </summary>
    public static BulletPattern CreatePatternInfo(BulletPatternInfo patternInfo, CharacterInfo.OwnerType ownerType)
    {
        return CreatePatternInfo(new BulletPatternEditInfo(patternInfo, 1, 0, false, false), ownerType);
    }

    /// <summary> bulletPatternEditInfo에서 bulletPatternInfo 클래스를 알맞은 클래스로 다운 캐스팅하고 bulletPattern을 생성하여 반환한다 </summary>
    public static BulletPattern CreatePatternInfo(BulletPatternEditInfo patternEditInfo, CharacterInfo.OwnerType ownerType)
    {
        System.Type bulletPatternType = patternEditInfo.patternInfo.GetType();

        // switch문으로 하려 했으나 에러 발생
        // c# 6 이전 버전에서는 switch식 또는 case 레이블은 bool, char, string, integral, enum 또는 해당하는 nullable 형식이어야 합니다.
        if (typeof(MultiDirPatternInfo) == bulletPatternType)
        {
            return new MultiDirPattern(patternEditInfo.patternInfo as MultiDirPatternInfo, patternEditInfo.executionCount, patternEditInfo.delay, patternEditInfo.isFixedOwnerDir, patternEditInfo.isFixedOwnerPos, ownerType);
        }
        else if (typeof(RowPatternInfo) == bulletPatternType)
        {
            return new RowPattern(patternEditInfo.patternInfo as RowPatternInfo, patternEditInfo.executionCount, patternEditInfo.delay, patternEditInfo.isFixedOwnerDir, patternEditInfo.isFixedOwnerPos, ownerType);
        }
        else if (typeof(LaserPatternInfo) == bulletPatternType)
        {
            return new LaserPattern(patternEditInfo.patternInfo as LaserPatternInfo, ownerType);
        }
        else if (typeof(SpreadPatternInfo) == bulletPatternType)
        {
            return new SpreadPattern(patternEditInfo.patternInfo as SpreadPatternInfo, patternEditInfo.executionCount, patternEditInfo.delay, patternEditInfo.isFixedOwnerDir, patternEditInfo.isFixedOwnerPos, ownerType);
        }
        else if (typeof(FixedOwnerPatternInfo) == bulletPatternType)
        {
            return new FixedOwnerPattern(patternEditInfo.patternInfo as FixedOwnerPatternInfo, ownerType);
        }
        else
        {
            return null;
        }
    }
}

public class ProjectilesPatternInfo : BulletPatternInfo
{
    [SerializeField]
    [Header("Saphe Pattern 필요할 때만 사용 bulletInfo 기준으로 배치되어 뿌려나갈 Child 총알들")]
    public List<ChildBulletInfo> childBulletInfoList;
    public ChildBulletCommonProperty childBulletCommonProperty;

    [Header("투사체 총알 정보")]
    public int bulletCount;     // 총알 갯수
}