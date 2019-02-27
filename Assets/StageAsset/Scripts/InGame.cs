using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.IN_GAME);
    }

    private void Start()
    {
            AudioManager.Instance.PlayMusic(4);
    }
}
