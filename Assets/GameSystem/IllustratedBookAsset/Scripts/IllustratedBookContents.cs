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

    // TODO : Item Book Contents 구현 해야 됨.
    public void Init(UsableItemInfo info)
    {
        contentsType = ContentsType.ITEM;
    }

    public void ShowContentsInfo()
    {
        switch(contentsType)
        {
            case ContentsType.WEAPON:
                // 임시
                BookContentsView.Instance.ShowContentsInfo(weaponInfo.weaponName);
                break;
            case ContentsType.ITEM:
                break;
            default:
                break;
        }
    }

    public void ActiveOffContents()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowContentsInfo();
    }
}
