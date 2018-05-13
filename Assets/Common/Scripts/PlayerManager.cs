using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager> {

    public GameObject playerPrefab;
    GameObject playerObj;
    GameObject m_playerObj;

    // 0513 모장현
    public Player GetPlayer()
    {
        return m_playerObj.GetComponent<Player>();
    }

    public Vector3 GetPlayerPosition()
    {
        if (m_playerObj == null)
            return Vector3.zero;
        return m_playerObj.transform.position;
    }

    public void SpawnPlayer()
    {
        m_playerObj = Instantiate(playerPrefab,RoomManager.Instance.RoomStartPoint(), Quaternion.identity);
        RoomManager.Instance.FindCurrentRoom(); // 플레이어 방찾기.
    }
    #region UnityFunc
    #endregion
}
