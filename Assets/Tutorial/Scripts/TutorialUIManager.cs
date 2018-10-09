using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tutorial의 UI를 관리하는 매니저 스크립트.
/// </summary>
public class TutorialUIManager : MonoBehaviourSingleton<TutorialUIManager>
{
    [SerializeField] Image[] layers;
    [SerializeField] GameObject textObj;

    public void FirstTest()
    {
        textObj.SetActive(true);
        TextUI.Instance.Test_Frist("move");
    }

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
