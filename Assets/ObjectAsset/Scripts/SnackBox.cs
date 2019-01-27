using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackBox : NoneRandomSpriteObject
{

    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        isAnimate = false;
        objectType = ObjectType.SNACKBOX;
    }

    public override void SetAvailable()
    {
        return;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            //stamina recovery
            isAvailable = false;
            sprite = sprites[1];
            spriteRenderer.sprite = sprite;
            Stamina.Instance.RecoverFullStamina();
            return true;
        }
        return false;
    }

    public override void IndicateInfo()
    {
        textMesh.text = "간식을 드시겠습니까?";
        childTextMesh.text = textMesh.text;
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
        childTextMesh.text = textMesh.text;
    }
}
