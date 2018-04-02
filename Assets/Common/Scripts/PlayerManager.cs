using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    public GameObject playerObj;

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
        return playerObj.transform.position;
    }
}
