using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviourSingleton<InGameManager> {

    #region UnityFunc
    private void Awake()
    {
        RoomSetManager.Instance.Init();
    }
    private void Start()
    {
        GenerateMap();
        SpawnPlayer();
        DrawUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            GoUpFloor();
    }
    #endregion
    #region Func
    public void GoUpFloor()
    {
        GameDataManager.Instance.SetFloor();
        GameDataManager.Instance.Savedata();
        //ObjectPoolManager.Instance.ClearWeapon();
        //ObjectPoolManager.Instance.ClearBullet();
        //ItemManager.Instance.DeleteObjs();

        GameDataManager.Instance.LoadData();
        GameStateManager.Instance.SetLoadsGameData(true);
        GameStateManager.Instance.LoadInGame();
        //PlayerManager.Instance.DeletePlayer();
        //UnityContext.DestroyClock();
        // ObjectPoolManager.Instance.ClearEffect();
        //Map.MapManager.Instance.GenerateMap(GameDataManager.Instance.GetFloor()); // 맵생성
        //SpawnPlayer();
        //DrawUI();
    } // 데이터 저장 타이밍
    void GenerateMap()
    {
        Map.MapManager.Instance.GenerateMap(GameDataManager.Instance.GetFloor()); // 맵생성
    }
    void SpawnPlayer()
    {
        PlayerManager.Instance.SpawnPlayer(); // 플레이어 스폰
    }
    void DrawUI()
    {
        UIManager.Instance.FadeInScreen(); // 화면 밝히기
        MiniMap.Instance.DrawMinimap(); // 미니맵 그리기
    }
    #endregion
}
