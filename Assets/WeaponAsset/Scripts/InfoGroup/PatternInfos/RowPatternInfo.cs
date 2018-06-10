using UnityEngine;


[CreateAssetMenu(fileName = "RowPatternInfo", menuName = "WeaponData/BulletPattern/RowPatternInfo", order = 1)]
public class RowPatternInfo : ProjectilesPatternInfo
{
    public float initPos;       // 초기 위치, 오른쪽 방향(0도)기준으로 시계방향 위치에서 반시계 방향 위치로 생성 -initPos 으로 계산됨
    public float deltaPos;      // 총알간의 위치 차이
    public float randomAngle;   // 총알 random값으로 각도 보정

    public RowPatternInfo()
    {
        // patternType = BulletPatternType.RowPattern;
    }

}
