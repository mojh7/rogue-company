﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviourSingleton<InGameManager> {

    int currentFloor = 0;
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
            CameraController.Instance.Shake(0.2f, .1f);
    }
    #endregion
    #region getter
    public int GetFloor()
    {
        return currentFloor;
    }
    #endregion
    #region Func
    public void GoUpFloor()
    {
        currentFloor++;
        ItemManager.Instance.DeleteObjs();
        PlayerManager.Instance.DeletePlayer();
        Map.MapManager.Instance.GenerateMap(currentFloor); // 맵생성
        SpawnPlayer();
        DrawUI();
    }
    void GenerateMap()
    {
        Map.MapManager.Instance.GenerateMap(currentFloor); // 맵생성
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