using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * 무기 이름, 공격력, 쿨타임, 탄약,
 * 
 * 
 */ 

public class BookContentsView : MonoBehaviourSingleton<BookContentsView>
{
    [SerializeField]
    private Text tempString;

    // Temp
    public void ShowContentsInfo(string tempStr)
    {
        tempString.text = tempStr;
    }
}
