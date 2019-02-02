using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
	void Awake () {
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.LOBBY);
    }
}
