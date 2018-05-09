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
<<<<<<< HEAD
        Map.MapManager.Getinstance().GenerateMap();
        PlayerManager.Getinstance().SpawnPlayer();
        RoomManager.Getinstance().FindCurrentRoom();
        UIManager.Instance.InitForPlayGame();   // 게임 첫 시작시 초기화
=======
        MapFunc();
        PlayerManager.Instance.SpawnPlayer();
        RoomManager.Instance.FindCurrentRoom();
>>>>>>> 9e3ef9440e3391ce2570a28dbbf9bfbae42245bd
    }
    #endregion

    void MapFunc()
    {
        Map.MapManager.Instance.GenerateMap();
    }
}
