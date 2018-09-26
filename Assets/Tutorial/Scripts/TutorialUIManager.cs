using UnityEngine;

public class TutorialUIManager : MonoBehaviour {

    private void Start()
    {
        SpawnPlayer();
        DrawUI();
    }

    void SpawnPlayer()
    {
        PlayerManager.Instance.FindPlayer(); // 플레이어 스폰
    }

    void DrawUI()
    {
        //UIManager.Instance.FadeInScreen(); // 화면 밝히기
    }
}
