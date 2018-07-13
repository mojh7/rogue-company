using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 효과 상세 내용으로 주로 적용 대상에 따라서 구분 됨.

// 아이템 상세한 사용효과 info
// 주로 적용 대상에 따른 구분


public abstract class ItemUseEffect : ScriptableObject
{
    [Tooltip("개발용 메모장")]
    [SerializeField]
    [TextArea(3, 100)] private string note;
}

