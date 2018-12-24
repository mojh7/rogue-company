using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AchievementAsset;

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
