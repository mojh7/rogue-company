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

    // 다들 값을 받아와서 UI에 text형태로 보여줄 값들(형변환 필요)
    #endregion

    #region function
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
        floorT.text = (5 + GameDataManager.Instance.GetFloor()).ToString() + "F";
        coinT.text = GameDataManager.Instance.GetCoin().ToString();
    }
    #endregion

    #region unityEngine
    public void Start()
    {
        LoadData();
    }
    #endregion
}
