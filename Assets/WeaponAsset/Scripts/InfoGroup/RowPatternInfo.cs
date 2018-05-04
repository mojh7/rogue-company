using UnityEngine;


[CreateAssetMenu(fileName = "RowPatternInfo", menuName = "GameData/RowPatternInfo", order = 2)]
public class RowPatternInfo : ScriptableObject
{
    public int bulletCount;     // 총알 갯수
    public float initPos;       // 초기 위치, 오른쪽 방향(0도)기준으로 시계방향 위치에서 반시계 방향 위치로 생성 -initPos 으로 계산됨
    public float deltaPos;      // 총알간의 위치 차이
    public float randomAngle;   // 총알 random값으로 각도 보정

    public int bulletId;        // 총알 Id
    public Sprite bulletSprite;  // 총알 sprite Id;

    [Tooltip("hahaha")]
    public float speed;         // 총알 속도
    public float range;         // 사정 거리
    public float damage;        // 총알 한 발 당 데미지

    [Tooltip("이 정보를 쓰고 있는 사람, 쓰이는 곳, 간단한 설명 등등 이것 저것 메모할 것들 적는 곳")]
    [SerializeField]
    [TextArea(3, 100)]
    private string memo;
}
