using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AchievementAsset;

//TODO : 업적 옵저버 패턴 기반으로 만들 듯?
// 업적으로만 얻을 수 있는 열리는 캐릭터, 특정 콘텐츠를 보상으로 하거나
// 공용으로 쓸 수 있는 골드를 주던가, 따로 업적 포인트를 줘서 이걸로만 살 수 있는 콘텐츠를 만들던가 등등

namespace AchievementAsset
{
    public enum AchievementType { COMMON, MONSTER, WEAPON, ITEM, MONEY }
}


[CreateAssetMenu(fileName = "Achievement", menuName = "GameSystem/Achievement", order = 1)]
public class Achievement : ScriptableObject
{
    public int achievementId;
    public string achievementName;
    public AchievementType type;
}
