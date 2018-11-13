using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviourSingleton<GameOverUI> {

    private delegate void ActiveOffAllPassiveSlot();

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
    private List<PassiveSlot> itemSlots;

    private ActiveOffAllPassiveSlot activeOffAllPassiveSlot;

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
        int itemCount = PlayerBuffManager.Instance.BuffManager.PassiveIds.Count;
        itemSlots = new List<PassiveSlot>(itemCount);
        itemPrefab.GetComponent<Image>().color = Color.clear;
        if (itemCount == 0)
            return;
        for (int i = 0; i < PlayerBuffManager.Instance.BuffManager.PassiveIds.Count; i++)
        {
            createdObj = Instantiate(itemPrefab);
            createdObj.name = "패시브 슬룻 ";
            createdObj.transform.position = currentPos;
            createdObj.transform.SetParent(contentsParent);
            activeOffAllPassiveSlot += createdObj.GetComponent<PassiveSlot>().ActiveOffPassiveSlot;
            createdObj.transform.localScale = new Vector3(1, 1, 1);
            itemSlots.Add(createdObj.GetComponent<PassiveSlot>());
        }
        activeOffAllPassiveSlot();

        for (int i = 0; i < PlayerBuffManager.Instance.BuffManager.PassiveIds.Count; i++)
        {
            itemSlots[i].UpdatePassiveSlot(PlayerBuffManager.Instance.BuffManager.PassiveIds[i]);
        }
    }
    #endregion

    #region unityEngine
    public void Awake()
    {
        Init();
        LoadData();
        GameDataManager.Instance.ResetData();
        Time.timeScale = 0;
    }
    #endregion
}
