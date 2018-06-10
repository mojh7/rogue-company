using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadPatternInfo", menuName = "WeaponData/BulletPattern/SpreadPatternInfo", order = 3)]
public class SpreadPatternInfo : ProjectilesPatternInfo
{

    public float sectorAngle;   // 부채꼴 각도
    public float randomAngle;   // 각 총알에서 random값으로 보정이 필요할 때.

    public SpreadPatternInfo()
    {
    }
}