using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitBox : NoneRandomSpriteObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        isAnimate = false;
        objectType = ObjectType.MEDKITBOX;
    }

    public override void SetAvailable()
    {
        return;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            //Item Drop
            isAvailable = false;
            sprite = sprites[1];
            spriteRenderer.sprite = sprite;
            return true;
        }
        return false;
    }

    public override void IndicateInfo()
    {
        textMesh.text = "약이 들어있습니다.";
        childTextMesh.text = textMesh.text;
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
        childTextMesh.text = textMesh.text;
    }
}
