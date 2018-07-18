using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSlot : MonoBehaviour
{
    [SerializeField]
    private Image passiveImage;

    public void UpdatePassiveSlot(Sprite sprite)
    {
        //if(null == sprite)
        //{
            passiveImage.sprite = sprite;
        //}
    }
}
