using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItem : CustomObject
{
    Item innerObject;
    Rating itemRating;
    int price;
    public override void Init()
    {
        base.Init();
        polygonCollider2D.isTrigger = true;
        isActive = false;
        isAvailable = true;
        isAnimate = true;
        objectType = ObjectType.STOREITEM;
        polygonCollider2D.SetPath(0, clickableBoxPolygon);
        SetAvailable();
        ShadowDrawing();
        gameObject.layer = 9;
    }

    public override void SetAvailable()
    {
        if (isAvailable)
        {
            Rating rating = ItemManager.Instance.GetStoreItemRating();
            innerObject = ObjectPoolManager.Instance.CreateUsableItem(rating);
            if (innerObject == null)
                return;
            itemRating = innerObject.GetRating();
            price = (int)(EconomySystem.Instance.GetPrice(itemRating) * (100 - PlayerBuffManager.Instance.BuffManager.InGameTargetEffectTotal.bargain) / 100);
            sprite = innerObject.GetComponent<SpriteRenderer>().sprite;
            ReAlign();
        }
    }

    void ReAlign()
    {
        innerObject.transform.parent = this.transform;
        innerObject.transform.localPosition = Vector3.zero;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            if (GameDataManager.Instance.GetCoin() >= price)
            {
                isAvailable = false;
                GameDataManager.Instance.ReduceCoin(price);
                ItemManager.Instance.CreateItem(innerObject, transform.position, new Vector2(Random.Range(-1, 2), 3));
            }
            return true;
        }
        return false;
    }

    public override void IndicateInfo()
    {
        if (!isAvailable)
            return;
        textMesh.text = innerObject.GetName() + " " + price;
        childTextMesh.text = textMesh.text;
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
        childTextMesh.text = textMesh.text;
    }

    public override void Delete()
    {
        base.Delete();
        if (innerObject == null)
            return;
        innerObject.transform.parent = null;
        innerObject.gameObject.SetActive(false);
    }
}

