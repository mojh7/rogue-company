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
    [SerializeField]
    private EffectApplyType[] statueBuffs;
    public void Awake()
    {
        // 게임 새로 시작 or 층 넘어 갈 때 or 로드 게임 구분 해야됨.
        // if()
        buffManager.Init();
    }

    #region func
    public void LoadMiscItemDatas()
    {
        List<int> miscItems = GameDataManager.Instance.GetMiscItems();
        for (int i = 0; i < miscItems.Count; i++)
        {
            buffManager.RegisterUsableItemInfo(DataStore.Instance.GetMiscItemInfo(miscItems[i]));
        }
    }
    #endregion

    #region npcBuffFunc

    public void ApplyAstrologerBuff()
    {
        if (astrologerBuffs == null || astrologerBuffs.Length == 0)
            return;
        int index = Random.Range(0, astrologerBuffs.Length);
        astrologerBuffs[index].UseItem();
    }

    public void ApplyStatueBuff(int idx)
    {
        if (statueBuffs == null || idx >= statueBuffs.Length)
            return;
        statueBuffs[idx].UseItem();
    }

    #endregion
}
