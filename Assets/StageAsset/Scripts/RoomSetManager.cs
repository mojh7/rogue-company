using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetManager : MonoBehaviour {

    private RoomSetManager instance;

    public RoomSetManager GetInstance()
    {
        if(instance == null)
        {
            instance = (RoomSetManager)Object.FindObjectOfType<RoomSetManager>();
        }
        return instance;
    }
 
}
