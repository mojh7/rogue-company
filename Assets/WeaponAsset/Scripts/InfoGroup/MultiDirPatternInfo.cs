using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiDirPatternInfo", menuName = "GameData/MultiDirPatternInfo", order = 1)]
public class MultiDirPatternInfo : ScriptableObject
{
    public int bulletCount;     // 총알 갯수
    public float initAngle;     // 초기 각도, -initAngle 으로 계산됨, 시계 방향
    public float deltaAngle;    // 총알간의 각도 차이
    public float randomAngle;   // 각 총알에서 random값으로 보정이 필요할 때.

    public int bulletId;        // 총알 Id
    public Sprite bulletSprite; // 총알 sprite;
    public int effectId;        // 이 패턴에서 나오는 모든 총알에 이펙트를 지정하고 싶으면 0 이상의 index 값으로 하고, 값이면 -1이면 bulletInfo 고유의 effect 생성

    [Tooltip("hahaha")]
    public float speed;         // 총알 속도
    public float range;         // 사정 거리
    public float damage;        // 총알 한 발 당 데미지

    [Tooltip("이 정보를 쓰고 있는 사람, 쓰이는 곳, 간단한 설명 등등 이것 저것 메모할 것들 적는 곳")]
    [SerializeField]
    [TextArea(3, 100)]
    private string memo;

    public MultiDirPatternInfo()
    {
        effectId = -1;
    }
}
