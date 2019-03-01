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
        int a = Random.Range(0, 2);
        if (a == 0)
            AudioManager.Instance.PlayMusic(4);
        else
            AudioManager.Instance.PlayMusic(9);
        //AudioManager.Instance.PlayMusic(4);
    }
}
