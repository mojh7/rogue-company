using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManager : MonoBehaviour {

    #region UnityFunc
    private void Awake()
    {
        RoomSetManager.GetInstance().Init();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
            Map.MapManager.Getinstance().GenerateMap();
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerManager.Getinstance().SpawnPlayer();
            RoomManager.Getinstance().FindCurrentRoom();
        }

    }
    private void Start()
    {
        Map.MapManager.Getinstance().GenerateMap();
        PlayerManager.Getinstance().SpawnPlayer();
        RoomManager.Getinstance().FindCurrentRoom();
    }
    #endregion
}
