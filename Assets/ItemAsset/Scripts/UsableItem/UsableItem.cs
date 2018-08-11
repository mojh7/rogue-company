using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 좋은 아이템 S ◁-------- E 덜 좋은 아이템 </summary>
public enum Rating { NORATING, S, A, B, C, D, E }

// item 상속 받는 중이라, item을 scriptable 할거 아니면 상속 2개 못해서
// info 클래스 따로 만들어서 scriptable 만들고 정보 관리

// 사용할 수 있는 아이템들
public class UsableItem : Item
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private UsableItemInfo info;

    /// <summary>
    /// info 및 sprite 초기화
    /// </summary>
    /// <param name="info"></param>
    public void Init(UsableItemInfo info)
    {
        this.info = info;
        this.value = info.Price;
        this.name = info.Name;
        spriteRenderer.sprite = info.Sprite;
    }

    public override void Active()
    {
        Debug.Log("UsableItem use");
        for(int i = 0; i < info.EffectApplyTypes.Length; i++)
        {
            info.EffectApplyTypes[i].UseItem();
        }
        ObjectPoolManager.Instance.DeleteUsableItem(gameObject);
    }
}

/*
// 음식 아이템
public class FoodItem : UsableItem
{
    public override void Active()
    {
        base.Active();
    }
}


// 의류 아이템
public class ClothingItem : UsableItem
{
    public override void Active()
    {
        base.Active();
    }
}

// 잡화(기타) 아이템
public class MiscItem : UsableItem
{
    public override void Active()
    {
        base.Active();
    }
}


// 의료 아이템
public class MedicalItem : UsableItem
{
    public override void Active()
    {
        base.Active();
    }
}

// 펫
public class PetItem : UsableItem
{
    public override void Active()
    {
        base.Active();
    }
}

// 패시브 아이템(특성 = 유물 = 등등)
public class PassiveItem : UsableItem
{
    public override void Active()
    {
        base.Active();
    }
}*/