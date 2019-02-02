using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    private void Awake()
    {
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.TITLE);
    }
    private void Start()
    {
        AudioManager.Instance.PlayMusic(0);
    }
}

