using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourSingleton<PlayerManager> {

    public GameObject playerPrefab;
    GameObject playerObj;
<<<<<<< HEAD

    public Joystick joystick;
    public static PlayerManager Getinstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
        }
        return instance;
    }
=======
>>>>>>> 9e3ef9440e3391ce2570a28dbbf9bfbae42245bd

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
