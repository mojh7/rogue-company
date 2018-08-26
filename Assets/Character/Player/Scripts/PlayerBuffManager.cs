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

    [Header("Npc에 따른 효과 적용")]
    [SerializeField]
    private EffectApplyType[] astrologerBuffs;
    public void Awake()
    {
        // 게임 새로 시작 or 층 넘어 갈 때 or 로드 게임 구분 해야됨.
        // if()
        buffManager.Init();
    }


    #region npcBuffFunc

    public void ApplyAstrologerBuff()
    {
        int index = Random.Range(0, astrologerBuffs.Length);
        astrologerBuffs[index].UseItem();
    }


    #endregion
}
