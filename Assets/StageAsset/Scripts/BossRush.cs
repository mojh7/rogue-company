using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRush : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
    {
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.BOSS_RUSH);
	}
}
