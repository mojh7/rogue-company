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
        TextUI.Instance.Test_Frist();
    }
}
