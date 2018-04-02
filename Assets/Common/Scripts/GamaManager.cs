using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManager : MonoBehaviour {

    #region UnityFunc
    private void Awake()
    {
        RoomSetManager.GetInstance().Init();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
            Map.MapManager.Getinstance().GenerateMap();
        if (Input.GetKeyDown(KeyCode.Q))
            RoomManager.Getinstance().DoorActive();

    }
#endregion
}
