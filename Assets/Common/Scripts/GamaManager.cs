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
            Map.MapManager.Instance.GenerateMap();
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerManager.Instance.SpawnPlayer();
            RoomManager.Instance.FindCurrentRoom();
        }

    }
    private void Start()
    {
        MapFunc();
        PlayerManager.Instance.SpawnPlayer();
        RoomManager.Instance.FindCurrentRoom();
    }
    #endregion

    void MapFunc()
    {
        Map.MapManager.Instance.GenerateMap();
    }
}
