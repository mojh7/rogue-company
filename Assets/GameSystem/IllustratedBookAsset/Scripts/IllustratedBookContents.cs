using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class IllustratedBookContents : MonoBehaviour, IPointerEnterHandler
{
    // 소모 아이템, 패시브 아이템 구분해야 할 듯?
    private enum ContentsType { WEAPON, ITEM }

    [SerializeField]
    protected Image image;
    [SerializeField]
    private Sprite emptySprite;
    private int slotId;
    private WeaponInfo weaponInfo;
    private UsableItemInfo usableItemInfo;

    private ContentsType contentsType;

    private void Awake()
    {
        image.sprite = emptySprite;
    }

    public void Init(WeaponInfo info)
    {
        contentsType = ContentsType.WEAPON;
        weaponInfo = info;
        image.sprite = weaponInfo.sprite;
    }

    public void Init(UsableItemInfo info)
    {
        contentsType = ContentsType.ITEM;
        usableItemInfo = info;
        image.sprite = info.Sprite;
    }

    public void ShowContentsInfo()
    {
        switch(contentsType)
        {
            case ContentsType.WEAPON:
                BookContentsView.Instance.ShowContentsInfo(weaponInfo);
                break;
            case ContentsType.ITEM:
                BookContentsView.Instance.ShowContentsInfo(usableItemInfo);
                break;
            default:
                break;
        }
    }

    public void SetActiveContents(bool show)
    {
        gameObject.SetActive(show);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowContentsInfo();
    }
}
