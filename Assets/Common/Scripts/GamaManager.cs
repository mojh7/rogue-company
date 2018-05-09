using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManager : MonoBehaviour {

    enum GameState { NOTSTARTED, GAMEOVER, PLAYING, CLEAR, ENDING }
    GameState gameState = GameState.NOTSTARTED;

    #region UnityFunc
    private void Awake()
    {
        RoomSetManager.GetInstance().Init();
    }

    private void Start()
    {
        UIManager.Instance.InitForPlayGame();   // 게임 첫 시작시 초기화
        GenerateMap();
        SpawnPlayer();
        DrawUI();
    }
    #endregion
    #region Func
    void GenerateMap()
    {
        Map.MapManager.Instance.GenerateMap(); // 맵생성
    }
    void SpawnPlayer()
    {
        PlayerManager.Instance.SpawnPlayer(); // 플레이어 스폰
    }
    void DrawUI()
    {
        UIManager.Instance.ToggleUI(); // UI 오픈
        MiniMap.Instance.DrawMinimap(); // 미니맵 그리기
    }
    #endregion
}
