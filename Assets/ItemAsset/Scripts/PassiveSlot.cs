using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image passiveImage;
    [SerializeField]
    private Sprite emptySprite;
    private int passiveId;
    private int slotId;
    private bool isTouchDown;

    private void Awake()
    {
        passiveId = -1;
        isTouchDown = false;
    }

    private void Update()
    {
        //if (isTouchDown)
        //{
        //    PassiveItemSlot.Instance.ShowPassiveInfoView(passiveId);
        //}
    }
    
    public void UpdatePassiveSlot(int passiveId)
    {
        this.passiveId = passiveId;
        passiveImage.sprite = ItemsData.Instance.GetMiscItemInfo(passiveId).Sprite;
    }

    public void ActiveOffPassiveSlot()
    {
        passiveImage.sprite = emptySprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (-1 != passiveId)
        {
            PassiveItemSlot.Instance.ShowPassiveInfoView(passiveId);
            isTouchDown = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isTouchDown)
        {
            PassiveItemSlot.Instance.ClosePassiveInfoView();
            isTouchDown = false;
        }
    }
}
