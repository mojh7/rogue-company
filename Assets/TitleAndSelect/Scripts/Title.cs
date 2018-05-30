using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    public GameObject RestartButton;

    private void Start()
    {
        if (GameDataManager.Instance.LoadData())
            RestartButton.SetActive(true);
    }

    public void LoadSelect()
    {
        GameStateManager.Instance.SetIsLoadedGame(false);
        GameDataManager.Instance.ResetData();
        GameStateManager.Instance.LoadSelect();
    }

    public void LoadInGame()
    {
        GameStateManager.Instance.SetIsLoadedGame(true);
        GameStateManager.Instance.LoadInGame();
    }
}

