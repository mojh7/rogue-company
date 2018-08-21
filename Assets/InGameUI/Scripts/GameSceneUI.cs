using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// MainScene 총 관리 하는 UI
/// </summary>
public class GameSceneUI : MonoBehaviour {
    
    private float currentTime;

    private void Awake()
    {
        currentTime = 0;
    }

    void Update () {
        if (Stamina.Instance.IsFullStamina())
        {
            currentTime += Time.deltaTime;
            if (currentTime > 3)
            {
                Stamina.Instance.RecoverStamina();
                currentTime = 0;
            }
        }
    }
    
    // 디버그용
    private void UpdateTimeData(float time)
    {
        string minutes = Mathf.FloorToInt(time / 60).ToString("00");
        string seconds = Mathf.FloorToInt(time % 60).ToString("00");
        string timeStr = minutes + " : " + seconds;
        //textT.text = "Time : " + timeStr;
    }
}
