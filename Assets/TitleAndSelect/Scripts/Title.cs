using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    public GameObject RestartButton;

    private void Start()
    {
        if (GameDataManager.Instance.GetFloor() > 0)
            RestartButton.SetActive(true);
    }

    public void LoadSelect()
    {
        GameStateManager.Instance.LoadSelect();
    }

    public void LoadInGame()
    {
        GameStateManager.Instance.LoadInGame();
    }
}

