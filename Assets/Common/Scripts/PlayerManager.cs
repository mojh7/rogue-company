using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager> {

    public GameObject playerPrefab;
    GameObject playerObj;

    public Vector3 GetPlayerPosition()
    {
        if (playerObj == null)
            return Vector3.zero;
        return playerObj.transform.position;
    }

    public void SpawnPlayer()
    {
        playerObj = Instantiate(playerPrefab,RoomManager.Instance.RoomStartPoint(), Quaternion.identity);
    }
    #region UnityFunc
    #endregion
}
