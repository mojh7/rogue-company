using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviourSingleton<GameOverUI> {

    #region var
    public Text timeT;
    public Text killT;
    public Text coinT;
    public Text rewardT;
    public Image[] floorI;

    [SerializeField] private Sprite[] arrow;
    public GameObject getItems;
    float playTime;
    int kill;
    int coin;
    int reward;
    int currentFloor;
    int preFloor;
    #endregion

    #region function
    public void Init()
    {
        playTime = GameDataManager.Instance.GetTime();
        kill = GameDataManager.Instance.GetKill();
        coin = GameDataManager.Instance.GetCoin();
        currentFloor = GameDataManager.Instance.GetFloor() + 6;
        preFloor = 10; // test용
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
        killT.text ="처치 수        " + kill.ToString();
        coinT.text = "획득 자금      " + coin.ToString();
        if (currentFloor != preFloor)
        {
            BuildingFloor(currentFloor, false);
            BuildingFloor(preFloor, true);
        }
        else
        {
            BuildingFloor(currentFloor, false);
        }
    }

    private void UpdateTimeData(float time)
    {
        string minutes = Mathf.FloorToInt(time / 60).ToString("00");
        string seconds = Mathf.FloorToInt(time % 60).ToString("00");
        string timeStr = minutes + " : " + seconds;
        timeT.text = "플레이 타임    " + timeStr;
    }

    private void BuildingFloor(int floor, bool isPre)
    {
        floorI[floor - 6].gameObject.SetActive(true);
        if (isPre)
            floorI[floor - 6].sprite = arrow[1];
        else
            floorI[floor - 6].sprite = arrow[0];
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
