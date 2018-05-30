using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiDirPatternInfo", menuName = "GameData/MultiDirPatternInfo", order = 1)]
public class MultiDirPatternInfo : ScriptableObject
{
    [Tooltip("적용하거나 쓰이는 곳, 사용하는 사람, 간단한 설명 등등 이것 저것 메모할 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    public int bulletCount;     // 총알 갯수
    public float initAngle;     // 초기 각도, -initAngle 으로 계산됨, 시계 방향
    public float deltaAngle;    // 총알간의 각도 차이
    public float randomAngle;   // 각 총알에서 random값으로 보정이 필요할 때.

    [Header("총알 ID !!!")]
    public int bulletId;        // 총알 Id

    public float speed;         // 총알 속도
    public float range;         // 사정 거리
    public float damage;        // 총알 한 발 당 데미지
    public float knockBack;     // 넉백 세기
    public float criticalRate;  // 크리티컬 확률
}
