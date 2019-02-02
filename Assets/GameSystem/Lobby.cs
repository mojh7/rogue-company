using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.LOBBY);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
