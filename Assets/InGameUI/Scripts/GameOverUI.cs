using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviourSingleton<GameOverUI> {

    #region var
    public Text timeT;
    public Text killT;
    public Text coinT;
    public Text floorT;
    public Text rewardT;

    public GameObject getItems;
    float playTime;
    int kill;
    int coin;
    int floor;
    int reward;

    // 다들 값을 받아와서 UI에 text형태로 보여줄 값들(형변환 필요)
    #endregion

    #region function
    public void Init()
    {
        playTime = GameDataManager.Instance.GetTime();
        kill = GameDataManager.Instance.GetKill();
        coin = GameDataManager.Instance.GetCoin();
        floor = GameDataManager.Instance.GetFloor();
        reward = 0;
    }
    public void LoadSelect()
    {
        GameStateManager.Instance.LoadSelect();
    }
    public void LoadTitle()
    {
        GameStateManager.Instance.LoadTitle();
    }
    public void LoadData()
    {
        UpdateTimeData(playTime);
        killT.text = kill.ToString() + " Kill!!";
        coinT.text = coin.ToString();
        floorT.text = (5 + floor).ToString() + "F";
    }
    private void UpdateTimeData(float time)
    {
        string minutes = Mathf.FloorToInt(time / 60).ToString("00");
        string seconds = Mathf.FloorToInt(time % 60).ToString("00");
        string timeStr = minutes + " : " + seconds;
        timeT.text = "Time : " + timeStr;
    }
    #endregion

    #region unityEngine
    public void Start()
    {
        Init();
        LoadData();
    }
    #endregion
}
