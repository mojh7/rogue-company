using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSlot : MonoBehaviour
{
    [SerializeField]
    private Image passiveImage;
    [SerializeField]
    private Sprite emptySprite;
    public void UpdatePassiveSlot(Sprite sprite)
    {
        passiveImage.sprite = sprite;
    }

    public void ActiveOffPassiveSlot()
    {
        passiveImage.sprite = emptySprite;
    }

}
