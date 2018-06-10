using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiDirPatternInfo", menuName = "WeaponData/BulletPattern/MultiDirPatternInfo", order = 0)]
public class MultiDirPatternInfo : ProjectilesPatternInfo
{
    public float initAngle;     // 초기 각도, -initAngle 으로 계산됨, 시계 방향
    public float deltaAngle;    // 총알간의 각도 차이
    public float randomAngle;   // 각 총알에서 random값으로 보정이 필요할 때.

    public  MultiDirPatternInfo()
    {
        // patternType = BulletPatternType.MultiDirPattern;
    }
}
