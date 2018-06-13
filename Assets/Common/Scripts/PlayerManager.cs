﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager>
{
    #region variables
    public GameObject playerPrefab;
    GameObject m_playerObj;
    Player m_player;
    public PlayerData[] playerDatas;

    #endregion

    #region getter
    // 0513 모장현
    public Player GetPlayer()
    {
        return m_player;
    }
    public Vector3 GetPlayerPosition()
    {
        if (m_playerObj == null)
            return Vector3.zero;
        return m_playerObj.transform.position;
    }
    #endregion

    #region function
    public void DeletePlayer()
    {
        // m_player.GetWeaponManager().RemoveWeapons();
        Destroy(m_playerObj);
    }

    public void SpawnPlayer()
    {
        m_playerObj = Instantiate(playerPrefab,RoomManager.Instance.RoomStartPoint(), Quaternion.identity);
        m_player = m_playerObj.GetComponent<Player>();
        m_player.Init();
        // 저장된 데이터 없이 새로운 게임을 시작할 때
        if (false == GameStateManager.Instance.GetLoadsGameData())
        {
            m_player.InitPlayerData(GameDataManager.Instance.GetPlayerData(Player.PlayerType.SOCCER));
        }
        // 저장된 데이터를 로드한 상태일 때
        else
        {
            m_player.InitPlayerData(GameDataManager.Instance.GetPlayerData());
        }
        GameStateManager.Instance.SetLoadsGameData(false);
        RoomManager.Instance.FindCurrentRoom(); // 플레이어 방찾기.
    }
    #endregion

    #region UnityFunc
    #endregion
}
