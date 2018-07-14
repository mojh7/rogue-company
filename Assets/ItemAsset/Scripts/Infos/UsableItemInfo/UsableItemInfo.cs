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

    // 이 두개 같은거 쓸 수도 있고 다른 것 쓸 수도 있음
    protected Sprite beforeActivedSprite;   // 사용 전 sprite로 아이템 박스에서 땅에 떨어져있는 상태에서의 sprite
    protected Sprite iconSprite;            // 버프 표시, 아이템 소유 표시 등 아이콘 모양의 sprite

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
}