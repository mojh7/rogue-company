using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBuffManager : MonoBehaviourSingleton<PlayerBuffManager>
{
    [SerializeField]
    private BuffManager buffManager;
    public BuffManager BuffManager
    {
        get { return buffManager; }
        set { buffManager = value; }
    }

    public void Awake()
    {
        // 게임 새로 시작 or 층 넘어 갈 때 or 로드 게임 구분 해야됨.
        // if()
        buffManager.Init();
    }
}
