using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager> {

    public GameObject playerPrefab;
    GameObject m_playerObj;
    Player m_player;

    

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

    public void DeletePlayer()
    {
        m_player.GetWeaponManager().RemoveWeapons();
        Destroy(m_playerObj);
    }

    public void SpawnPlayer()
    {
        m_playerObj = Instantiate(playerPrefab,RoomManager.Instance.RoomStartPoint(), Quaternion.identity);
        m_player = m_playerObj.GetComponent<Player>();
        RoomManager.Instance.FindCurrentRoom(); // 플레이어 방찾기.
    }

    #region UnityFunc
    #endregion
}
