using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
