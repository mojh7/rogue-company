using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviourSingleton<GameOverUI> {

    #region components
    [SerializeField]
    Text timeT;
    [SerializeField]
    Text killT;
    [SerializeField]
    Text coinT;
    [SerializeField]
    Text rewardT;
    [SerializeField]
    Image[] floorI;
    #endregion
    #region var
    [SerializeField]
    Sprite arrow;

    float playTime;
    int kill;
    int coin;
    int reward;
    int currentFloor;
    int preFloor;
    #endregion
    #region forItem
    [SerializeField]
    private Transform contentsParent;
    [SerializeField]
    private GameObject itemPrefab;
    //private List<PassiveSlot> itemSlots;

    #endregion
    #region function
    public void Init()
    {
        TimeController.Instance.StopTime();
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
        CreatePassiveSlots();
        killT.text ="처치 수       " + kill.ToString();
        coinT.text = "획득 자금     " + coin.ToString();
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
        timeT.text = "플레이 타임   " + timeStr;
    }

    private void BuildingFloor(int floor, bool isPre)
    {
        floorI[floor - 6].gameObject.SetActive(true);
        if (isPre)
        {
            floorI[floor - 6].color = new Color(.5f, .5f, .5f);  
        }
        else
        {
            floorI[floor - 6].color = Color.red;
        }
    }

    private void CreatePassiveSlots()
    {
        GameObject createdObj;
        Vector3 currentPos = Vector3.zero;
        int passiveItemCount = PlayerBuffManager.Instance.BuffManager.PassiveIds.Count;
        //itemSlots = new List<PassiveSlot>(passiveItemCount);
        itemPrefab.GetComponent<Image>().color = Color.clear;
        if (passiveItemCount == 0)
            return;
        for (int i = 0; i < passiveItemCount; i++)
        {
            createdObj = Instantiate(itemPrefab);
            createdObj.name = "결과창 패시브 아이템_" + i;
            createdObj.transform.position = currentPos;
            createdObj.transform.SetParent(contentsParent);
            createdObj.transform.localScale = new Vector3(1, 1, 1);
            createdObj.GetComponent<PassiveSlot>().UpdatePassiveSlot(PlayerBuffManager.Instance.BuffManager.PassiveIds[i]);
        }

        for (int i = 0; i < PlayerBuffManager.Instance.BuffManager.PassiveIds.Count; i++)
        {
            
        }
    }
    #endregion

    #region unityEngine
    public void Awake()
    {
        Init();
        LoadData();
        GameDataManager.Instance.ResetData(GameDataManager.UserDataType.INGAME);
        Time.timeScale = 0;
    }
    #endregion
}
