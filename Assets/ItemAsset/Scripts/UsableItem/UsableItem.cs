using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 어떤 형태로든 캐릭터 스탯이나 무기에 효과를 주는 아이템들

// usableItem은 Player만 쓸 수 있도록 생각해서 만들 듯

/* 아이템 종류 중
 * 사용 가능한 아이템이 있다.
 * 그 종류는 5가지이며 음식, 의류, 잡화, 의료, 펫 아이템이 있다.
 * 
 * [공통점]
 * 모두 습득 즉시 사용되어 효과를 얻을 수 있다.
 * 
 * [차이점]
 * 효과가 적용될 때 적용되는 방법에 차이가 나는데 3가지가 있다.
 * 
 * 효과 적용 방법
 * 
 * 
 * 
 */ 


// public enum UsableItemUseType { CONSUMABLE, BUFF, PASSIVE, SPECIAL }

/// <summary> 좋은 아이템 S </<>-------- E 덜 좋은 아이템 </summary>
public enum Rating { S, A, B, C, D, E }

// 사용할 수 있는 아이템들
public abstract class UsableItem : Item {

    protected Rating rating;
    protected string name;
    protected string notes;
    protected int price;

    protected List<UseMethod> useMethods;
    protected int useMethodsLength;


    // 이 두개 같은거 쓸 수도 있고 다른 것 쓸 수도 있음
    protected Sprite beforeActivedSprite;   // 사용 전 sprite로 아이템 박스에서 땅에 떨어져있는 상태에서의 sprite
    protected Sprite iconSprite;            // 버프 표시, 아이템 소유 표시 등 아이콘 모양의 sprite

    /* virtual로 해야 될 듯?
    protected override void Active()
    {
        Debug.Log("UsableItem use");
        for(int i = 0; i < itemEffectsLength; i++)
        {
            itemEffects[i].useItem();
        }
    }
    */
}

// 음식 아이템
public class FoodItem : UsableItem
{
    public override void Active()
    {
        throw new System.NotImplementedException();
    }
}

// 의류 아이템
public class ClothingItem : UsableItem
{
    public override void Active()
    {
        throw new System.NotImplementedException();
    }
}

// 잡화(기타) 아이템
public class MiscItem : UsableItem
{
    public override void Active()
    {
        throw new System.NotImplementedException();
    }
}


// 의료 아이템
public class MedicalItem : UsableItem
{
    public override void Active()
    {
        throw new System.NotImplementedException();
    }
}

// 펫
public class PetItem : UsableItem
{
    public override void Active()
    {
        throw new System.NotImplementedException();
    }
}