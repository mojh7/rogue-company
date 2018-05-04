using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private static PlayerManager instance;
    public GameObject playerPrefab;
    GameObject playerObj;
    public static PlayerManager Getinstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
        }
        return instance;
    }

    public Vector3 GetPlayerPosition()
    {
        if (playerObj == null)
            return Vector3.zero;
        return playerObj.transform.position;
    }

    public void SpawnPlayer()
    {
        playerObj = Instantiate(playerPrefab,RoomManager.Getinstance().RoomStartPoint(), Quaternion.identity);
    }
    #region UnityFunc
    #endregion
}
