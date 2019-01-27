using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnbreakableBox : RandomSpriteObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.UNBREAKABLE;
    }
    public override void SetAvailable()
    {
    }
}
