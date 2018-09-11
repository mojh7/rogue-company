using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Image passiveImage;
    [SerializeField]
    private Sprite emptySprite;
    private int passiveId;

    private bool isTouchDown;

    private void Awake()
    {
        passiveId = -1;
        isTouchDown = false;
    }

    private void Update()
    {
        if (isTouchDown)
        {
            PassiveItemSlot.Instance.ShowPassiveInfoView(passiveId);
        }
    }

    public void UpdatePassiveSlot(int passiveId)
    {
        this.passiveId = passiveId;
        passiveImage.sprite = DataStore.Instance.GetMiscItemInfo(passiveId).Sprite;
    }

    public void ActiveOffPassiveSlot()
    {
        passiveImage.sprite = emptySprite;
    }

    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(name + ", " + passiveId);
        if(-1 != passiveId)
        {
            // PassiveItemSlot.Instance.ShowPassiveInfoView(passiveId);
            isTouchDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(isTouchDown)
        {
            PassiveItemSlot.Instance.ClosePassiveInfoView();
            isTouchDown = false;
        }
    }
}
