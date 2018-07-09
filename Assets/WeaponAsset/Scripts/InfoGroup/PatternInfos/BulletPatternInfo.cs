using System;
using UnityEngine;

// clone 해주는 함수 없으나 필요하다고 생각 하면 넣을 예정.

public class BulletPatternInfo : ScriptableObject
{
    [Tooltip("적용하거나 쓰이는 곳, 사용하는 사람, 간단한 설명 등등 이것 저것 메모할 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    // weapon Info에서 다운 캐스팅 할 때 pattern type 구분용
    // public BulletPatternType patternType;

    [Tooltip("총알 Info, laser 무기는 꼭 laserUpdate와 laserDelete 속성을 가진 총알 만 쓰기")]
    public BulletInfo bulletInfo;

    /// <summary> bulletPatternInfo 클래스를 알맞은 클래스로 다운 캐스팅하고 bulletPattern을 생성하여 반환한다 </summary>
    public static BulletPattern CreatePatternInfo(BulletPatternInfo patternInfo, CharacterInfo.OwnerType ownerType)
    {
        return CreatePatternInfo(new BulletPatternEditInfo(patternInfo, 1, 0), ownerType);
    }

    /// <summary> bulletPatternEditInfo에서 bulletPatternInfo 클래스를 알맞은 클래스로 다운 캐스팅하고 bulletPattern을 생성하여 반환한다 </summary>
    public static BulletPattern CreatePatternInfo(BulletPatternEditInfo patternEditInfo, CharacterInfo.OwnerType ownerType)
    {
        System.Type bulletPatternType = patternEditInfo.patternInfo.GetType();

        // switch문으로 하려 했으나 에러 발생
        // c# 6 이전 버전에서는 switch식 또는 case 레이블은 bool, char, string, integral, enum 또는 해당하는 nullable 형식이어야 합니다.
        if (typeof(MultiDirPatternInfo) == bulletPatternType)
        {
            return new MultiDirPattern(patternEditInfo.patternInfo as MultiDirPatternInfo, patternEditInfo.executionCount, patternEditInfo.delay, ownerType);
        }
        else if (typeof(RowPatternInfo) == bulletPatternType)
        {
            return new RowPattern(patternEditInfo.patternInfo as RowPatternInfo, patternEditInfo.executionCount, patternEditInfo.delay, ownerType);
        }
        else if (typeof(LaserPatternInfo) == bulletPatternType)
        {
            return new LaserPattern(patternEditInfo.patternInfo as LaserPatternInfo, ownerType);
        }
        else if (typeof(SpreadPatternInfo) == bulletPatternType)
        {
            return new SpreadPattern(patternEditInfo.patternInfo as SpreadPatternInfo, patternEditInfo.executionCount, patternEditInfo.delay, ownerType);
        }
        else
        {
            return null;
        }
    }
}

public class ProjectilesPatternInfo : BulletPatternInfo
{
    [Header("투사체 총알 정보")]
    public int bulletCount;     // 총알 갯수
}