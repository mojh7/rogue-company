using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    public GameObject RestartButton;

    private void Start()
    {
        if (GameDataManager.Instance.GetGameData() != null)
            RestartButton.SetActive(true);
    }

    public void LoadSelect()
    {
        GameDataManager.Instance.ResetData();
        GameStateManager.Instance.LoadSelect();
    }

    public void LoadInGame()
    {
        GameStateManager.Instance.LoadInGame();
    }
}

