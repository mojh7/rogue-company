using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : RandomSpriteObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.PORTAL;
        gameObject.layer = 9;
    }

    public override void SetAvailable()
    {
    }
    public void Possible()
    {
        isAvailable = true;
    }
    public override bool Active()
    {
        if (!isAvailable)
            return false;
        base.Active();
        isAvailable = false;
        InGameManager.Instance.GoUpFloor();

        return true;
    }
}
