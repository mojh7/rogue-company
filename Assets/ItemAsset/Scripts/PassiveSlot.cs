using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Image passiveImage;
    [SerializeField]
    private Sprite emptySprite;
    private int passiveId;
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
        PassiveItemSlot.Instance.ShowPassiveInfoView(passiveId);
    }
}
