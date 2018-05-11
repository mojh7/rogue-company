using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{

    private GameObject[] hpObjList;
    private GameObject overHpTextObj; // 추가 텍스트 표시 Object;
    private Text overHpText;          // 추가 텍스트
    private Image[] hpImageList;

    [SerializeField]
    private GameObject hpPrefab;    // hp object Prefab
    [SerializeField]
    private GameObject overHpTextPrefab;    // overHpText object Prefab

    [SerializeField]
    private Sprite hpFullSprite;
    [SerializeField]
    private Sprite hpHalfSprite;

    private float deltaX; // 체력 obj 간에 x값 차이
    private int column;         // 하트모양의 체력sprite 한 줄 최대 표현 갯수
    private string columnStr;   // int형 column을 string으로 저장해놓고 씀.
    private float deltaPosX; // hp object 간의 position x값 차이
    private Vector3 hpPos;

    private Transform objTransform;



    private float overHp;
    private float hpForDebug;


    // Use this for initialization
    void Start()
    {
        objTransform = GetComponent<Transform>();
        column = 10;
        deltaPosX = 60f;
        Init();
        hpForDebug = 7.5f;
        UpdateHPUI(hpForDebug);
    }

    /// <summary>
    /// Player HP UI 초기화, 한 줄에 최대 표현 갯수 만큼 heart Object 생성 및 object, Image, TextObject 정보 캐싱
    /// </summary>
    public void Init()
    {
        // 필요한 정보들 미리 캐싱 후 사용
        hpObjList = new GameObject[column];
        hpImageList = new Image[column];

        hpPos = objTransform.position;

        for (int i = 0; i < column; i ++)
        {
            hpObjList[i] = Instantiate(hpPrefab);
            hpObjList[i].GetComponent<Transform>().SetParent(objTransform);
            hpObjList[i].GetComponent<Transform>().position = hpPos;
            hpImageList[i] = hpObjList[i].GetComponent<Image>();
            hpPos.x += deltaPosX;
        }

        hpPos.x += 1.3f * deltaPosX;
        // hp 최대 표시 갯수를 넘은 추가 hp 표시 object 생성
        overHpTextObj = Instantiate(overHpTextPrefab);
        overHpTextObj.GetComponent<Transform>().SetParent(objTransform);
        overHpTextObj.GetComponent<Transform>().position = hpPos;
        overHpText = overHpTextObj.GetComponent<Text>();
    }

    void Update()
    {
        // 아직 디버그용으로 player hp랑 연결 안 시켜 놓음
        // 디버그용 hp + 0.5f;
        if(Input.GetKeyDown(KeyCode.P))
        {
           hpForDebug += 0.5f;
           UpdateHPUI(hpForDebug);
        }

        // 디버그용 hp - 0.5f;
        if (Input.GetKeyDown(KeyCode.O))
        {
            hpForDebug -= 0.5f;
            UpdateHPUI(hpForDebug);
        }
    }


    // hp ui 갱신
    public void UpdateHPUI(float hp)
    {
        if (hp < 0) return;

        int remainder = (int)hp % column;

        Debug.Log("hp : " + hp + ", rema : " + remainder);

        // hp가 최대 표시 갯수를 넘을 때
        if (hp >= column)
        {
            overHpTextObj.SetActive(false);
            for (int i = 0; i < column; i++)
            {
                hpObjList[i].SetActive(true);
                hpImageList[i].sprite = hpFullSprite;
            }
            if(hp > column)
            {
                overHpTextObj.SetActive(true);
                overHp = hp - column;
                overHpText.text = "+ " + overHp.ToString();
            }
        } 
        else // column 수 이하 일 때 체력 ui 표시
        {
            overHpTextObj.SetActive(false);
            for (int i = 0; i < remainder; i++)
            {
                hpObjList[i].SetActive(true);
                hpImageList[i].sprite = hpFullSprite;

            }
            for (int i = remainder; i < column; i++)
            {
                hpObjList[i].SetActive(false);
            }

            // 0.5 소수 유무 판단 후 hp half 표시
            if (hp - (int)hp == 0.5)
            {
                hpObjList[remainder].SetActive(true);
                hpImageList[remainder].sprite = hpHalfSprite;
            }
        }
        
        
    }
}
