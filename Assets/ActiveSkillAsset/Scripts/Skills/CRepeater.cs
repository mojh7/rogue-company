using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CRepeater", menuName = "SkillData/CRepeater")]
public class CRepeater : SkillData
{
    [SerializeField]
    List<SkillData> skillData;
}
