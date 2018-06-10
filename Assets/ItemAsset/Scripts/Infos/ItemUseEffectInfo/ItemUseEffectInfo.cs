using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemUseEffectInfo : ScriptableObject
{
    [Tooltip("개발용 메모장")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;
}