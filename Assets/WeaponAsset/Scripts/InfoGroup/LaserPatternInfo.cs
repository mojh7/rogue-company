using UnityEngine;

[CreateAssetMenu(fileName = "LaserPatternInfo", menuName = "GameData/LaserPatternInfo", order = 3)]
public class LaserPatternInfo : ScriptableObject
{
    public int bulletId;        // 총알 Id, laserUpdate와 laserDelete 속성을 꼭 가진 총알 만
    public int damage;

    [Tooltip("이 정보를 쓰고 있는 사람, 쓰이는 곳, 간단한 설명 등등 이것 저것 메모할 것들 적는 곳")]
    [SerializeField]
    [TextArea(3, 100)]
    private string memo;
}
