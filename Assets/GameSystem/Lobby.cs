using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviourSingleton<Lobby>
{
    [SerializeField]
    private Text goldTxt;

	void Awake ()
    {
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.LOBBY);
    }

    private void Start()
    {
        UpdateGoldUI(GameDataManager.Instance.GetGold());
    }

    public void UpdateGoldUI(int gold)
    {
        goldTxt.text = gold.ToString();
    }

    public void UseMoney()
    {
        GameDataManager.Instance.UseMoeny(100);
    }

    public void MakeMoney()
    {
        GameDataManager.Instance.MakeMoeny(123);
    }
}
