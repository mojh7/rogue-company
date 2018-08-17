using UnityEngine;


[CreateAssetMenu(fileName = "RowPatternInfo", menuName = "WeaponData/BulletPattern/RowPatternInfo", order = 1)]
public class RowPatternInfo : ProjectilesPatternInfo
{
    [Header("몇 번 deltaAngle 만큼 회전하는지에 대한 값")]
    public int rotatingCount;
    [Header("초기 위치, 반시계 방향")]
    public float initPos;       // 초기 위치, 오른쪽 방향(0도)기준으로 시계방향 위치에서 반시계 방향 위치로 생성 -initPos 으로 계산됨
    [Header("총알 마다 위치 차이, 시계 방향")]
    public float deltaPos;      // 총알간의 위치 차이
    public float randomAngle;   // 총알 random값으로 각도 보정
    public float initAngle;     // 초기 각도, -initAngle 으로 계산됨, 시계 방향
    public float deltaAngle;    // 총알간의 각도 차이
    public RowPatternInfo()
    {
        rotatingCount = 1;
        // patternType = BulletPatternType.RowPattern;
    }

}
