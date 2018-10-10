using System;
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
    [HideInInspector] public GameObject obj;

    public void FirstTest()
    {
        textObj.SetActive(true);
        TextUI.Instance.Test_Frist("move");
    }

    // 1010 focus 동적할당 테스트
    public void SetFocusImage()
    {
        obj = Instantiate(focusImage);
        obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RectTransform im = obj.GetComponent<RectTransform>();
        im.position = new Vector2(layers[0].rectTransform.position.x, layers[0].rectTransform.position.y);
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
}
