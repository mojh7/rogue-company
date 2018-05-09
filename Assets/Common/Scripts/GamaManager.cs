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
        UIManager.Instance.InitForPlayGame();   // 게임 첫 시작시 초기화
        Map.MapManager.Instance.GenerateMap(); // 맵생성
        PlayerManager.Instance.SpawnPlayer(); // 플레이어 스폰
        UIManager.Instance.ToggleUI(); // UI 오픈
        MiniMap.Instance.DrawMinimap(); // 미니맵 그리기
    }
    #endregion
}
