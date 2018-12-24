using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: 보상은 ScriptableObject 안하고 그냥 구조체나 클래스로도 바뀔 수 있음. 혹은 보상 상위 클래스를 놓고 상속받아 상점, 업적 등 다양한 보상 클래스 만들던가, 매니저격 클래스 만들고 공용으로 쓰던가


[CreateAssetMenu(fileName = "AchievementReward", menuName = "GameSystem/Achievement", order = 2)]
public class AchievementReward : ScriptableObject {

}
