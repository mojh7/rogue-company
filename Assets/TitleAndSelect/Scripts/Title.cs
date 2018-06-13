using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    public GameObject RestartButton;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(0);
        if (GameDataManager.Instance.LoadData())
            RestartButton.SetActive(true);
    }

    // start
    public void LoadSelect()
    {
        GameStateManager.Instance.SetLoadsGameData(false);
        GameDataManager.Instance.ResetData();
        GameStateManager.Instance.LoadSelect();
    }

    // restart
    public void LoadInGame()
    {
        GameStateManager.Instance.SetLoadsGameData(true);
        GameStateManager.Instance.LoadInGame();
    }
}

