using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    private void Start()
    {
        SpawnPlayer();
        DrawUI();

        StartCoroutine("First");
    }

    void SpawnPlayer()
    {
        PlayerManager.Instance.FindPlayer(); // 플레이어 스폰
    }

    void DrawUI()
    {
        UIManager.Instance.FadeInScreen(); // 화면 밝히기
    }

    IEnumerator First()
    {
        yield return new WaitForSeconds(1.5f);
        TutorialUIManager.Instance.FirstTest();
        yield return null;
    }
}
