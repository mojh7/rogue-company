using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviourSingleton<GameOverUI> {

    public Text timeT;
    public Text killT;
    public Text coinT;
    public Text floorT;
    public Text rewardT;

    public GameObject getItems;

    public void LoadSelect()
    {
        GameStateManager.Instance.LoadSelect();
    }
    public void LoadTitle()
    {
        GameStateManager.Instance.LoadTitle();
    }
}
