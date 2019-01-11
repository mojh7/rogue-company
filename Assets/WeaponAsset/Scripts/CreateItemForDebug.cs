using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class CreateItemForDebug : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        UsableItem usableItem = ObjectPoolManager.Instance.CreateUsableItem();
        usableItem.Init(ItemsData.Instance.GetMiscItemInfo(Random.Range(0, ItemsData.Instance.GetMiscItemInfosLength())));
        ItemManager.Instance.CreateItem(usableItem, PlayerManager.Instance.GetPlayerPosition());
    }
}
    
