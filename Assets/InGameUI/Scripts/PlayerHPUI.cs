using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPUI : MonoBehaviour
{

    [SerializeField]
    private GameObject[] hpObjList;
    private GameObject[] hpTextObjList;

    private float deltaX; // 체력 obj 간에 x값 차이
    private int column; // 열 갯수

    [SerializeField]
    private Sprite hpFullSprite;
    private Sprite hpHalfSprite;
    // Use this for initialization
    void Start()
    {

    }

    //7.5
    // hp ui 갱신
    public void UpdateHPUI(int hp)
    {
        int remainder = hp % 10;
        for(int i = 0; i < remainder; i++)
        {
            hpObjList[i].SetActive(true);
        }
        for(int i = remainder; i < column; i++)
        {
            hpObjList[i].SetActive(false);
        }
    }
}
