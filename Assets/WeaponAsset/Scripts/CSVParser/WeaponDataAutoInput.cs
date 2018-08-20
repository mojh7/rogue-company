using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class WeaponDataAutoInput : MonoBehaviourSingleton<WeaponDataAutoInput>
{
    [Header("true하고 실행 시 엑셀 내용으로 무기 초기화")]
    [SerializeField]
    private bool canInputDatas;

    public List<Dictionary<string, object>> datas;
    // Use this for initialization
    void Start () {
	}
}


