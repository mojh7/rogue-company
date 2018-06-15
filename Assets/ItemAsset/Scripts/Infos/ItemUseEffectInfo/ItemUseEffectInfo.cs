using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 상세한 사용효과 info
// 주로 적용 대상에 따른 구분

public class ItemUseEffectInfo : ScriptableObject
{
    [Tooltip("개발용 메모장")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;
}