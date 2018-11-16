using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tutorial의 UI를 관리하는 매니저 스크립트.
/// </summary>
public class TutorialUIManager : MonoBehaviourSingleton<TutorialUIManager>
{
    [SerializeField] Image[] layers;
    [SerializeField] GameObject textObj;
    [SerializeField] GameObject focusImage;
    [SerializeField] GameObject[] menuObj;   

    [SerializeField] Image focus;
    [HideInInspector] public GameObject obj;

    public int count = 0;

    public void FirstTest()
    {
        textObj.SetActive(true);
        switch (count)
        {
            case 0:
                TextUI.Instance.Test_Frist("move");
                break;
            case 1:
                // 무기 주우면 실행
                TextUI.Instance.Test_Frist("swap");
                break;
            case 2:
                // TextUI.족 AI 활성화
                TextUI.Instance.Test_Frist("enemy");
                break;
            case 3:
                // 몬스터가 한 방 맞으면 활성화
                TextUI.Instance.Test_Frist("attack");
                break;
            case 4:
                TextUI.Instance.Test_Frist("clear");
                break;
        }
    }

    // 1010 focus 동적할당 테스트 -> 사용 xx
    public void SetFocusImage()
    {
        Debug.Log(1234);
        obj = Instantiate(focusImage);
        obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
        obj.SetActive(true);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.position = new Vector2(layers[0].rectTransform.position.x, layers[0].rectTransform.position.y);
    }
    
    public void SetFocus(int i)
    {
        focus.rectTransform.position = layers[i].rectTransform.position;

        CircleAnimation.Instance.isTrue = false;
        focus.gameObject.SetActive(true);
    }

    public void SetFocus(GameObject obj)
    {
        focus.rectTransform.position = obj.GetComponent<Image>().rectTransform.position;

        //CircleAnimation.Instance.isTrue = false;
        focus.gameObject.SetActive(true);
    }

    // layer 하나하나 active 설정하기
    public void SetLayersActive(int i, bool act)
    {
        layers[i].gameObject.SetActive(act);
    }

    // 클릭 못하게 layer 들여내기
    public void HoldOn()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].gameObject.SetActive(true);
        }
    }

    // 원하는 index 순서까지 layer 뺴기
    // layer 순서 : 이동 조이스틱 -> 공격 조이스틱 -> 설정창 -> 구르기 -> 무기스와이프
    public void HoldOut(int index)
    {
        for (int i = 0; i < index; i++)
        {
            layers[i].gameObject.SetActive(false);
        }
    }

    IEnumerator ActiveTrueMenu()
    {
        layers[5].gameObject.SetActive(true);
        //SetFocus(menuObj[0]);
        yield return new WaitForSeconds(1.5f);
        menuObj[0].SetActive(true);
        //SetFocus(menuObj[1]);
        yield return new WaitForSeconds(1.5f);
        menuObj[1].SetActive(true);
        //SetFocus(세미뭐시기);
        yield return new WaitForSeconds(1.5f);
        FirstTest();
        yield return new WaitForSeconds(1.5f);
        layers[5].gameObject.SetActive(false);
        menuObj[0].SetActive(false); menuObj[1].SetActive(false);
    }

    public void HoldAll(bool isActive)
    {
        layers[5].gameObject.SetActive(isActive);
    }
}
