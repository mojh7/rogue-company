using UnityEngine;


[CreateAssetMenu(fileName = "LaserPatternInfo", menuName = "GameData/LaserPatternInfo", order = 3)]
public class LaserPatternInfo : ScriptableObject
{
    [Tooltip("적용하거나 쓰이는 곳, 사용하는 사람, 간단한 설명 등등 이것 저것 메모할 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    public int bulletId;        // 총알 Id, laserUpdate와 laserDelete 속성을 꼭 가진 총알 만
    public int damage;
}
