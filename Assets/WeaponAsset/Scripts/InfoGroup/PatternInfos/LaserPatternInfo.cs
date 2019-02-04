using UnityEngine;


[CreateAssetMenu(fileName = "LaserPatternInfo", menuName = "WeaponData/BulletPattern/LaserPatternInfo", order = 2)]
public class LaserPatternInfo : BulletPatternInfo
{
    public float additionalDirDegree;
    public LaserPatternInfo()
    {
        // patternType = BulletPatternType.LaserPattern;
    }
}
