using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partition : UnbreakableBox
{
    public override void Init()
    {
        base.Init();
        spriteRenderer.sortingLayerName = "Background";
        spriteRenderer.sortingOrder = 3;
        objectType = ObjectType.PARTITION;
    }
}
