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
        int a = Random.Range(0, 2);
        if(a == 0)
            AudioManager.Instance.PlayMusic(0);
        else
            AudioManager.Instance.PlayMusic(8);
    }
}

