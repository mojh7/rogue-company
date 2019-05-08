using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UsableItemInfo
// 푸드, 잡화, 의류 등 이런거 딱히 차이가 없고 효과 차이 어차피 다른 클래스들 UseMethod랑 ItemUseEffect 바꿔주면 되서
// 굳이 UsableItem이랑 UsableItemInfo 자식 클래스로 나누어야 하나 생각 듬. enum 하나 만들어서 type 만 바꿔줘도 될 것 같은데
public class UsableItemInfo : ScriptableObject
{
    [Tooltip("개발용 메모장")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    [SerializeField]
    protected int id;
    [SerializeField]
    protected Rating rating;
    [SerializeField]
    protected string itemName;
    [SerializeField]
    [TextArea(3, 50)]
    [Header("아이템 설명")]
    protected string notes; // 설명
    [SerializeField]
    protected int price;
    [SerializeField]
    protected Sprite sprite;

    [SerializeField]
    protected EffectApplyType[] effectApplyTypes;

    public void SetId(int id)
    {
        this.id = id;
    }

    public int GetId()
    {
        return id;
    }

    public EffectApplyType[] EffectApplyTypes
    {
        get { return effectApplyTypes; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
    }

    public string Notes
    {
        get { return notes; }
    }

    public int Price
    {
        get { return price; }
    }

    public string ItemName
    {
        get { return itemName; }
    }

    public Rating Rating
    {
        get { return rating; }
    }

    public string Memo
    {
        get { return memo; }
    }
}