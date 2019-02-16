using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakalbeBox : RandomSpriteObject
{
    int duration;
    const int durationMax = 3;
    public override void SetAvailable()
    {
    }

    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.BREAKABLE;
        duration = durationMax;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UtilityClass.CheckLayer(collision.gameObject.layer, 15, 17) && duration > 0)
        {
            duration--;
            spriteRenderer.color = new Color(1, (float)duration / durationMax, (float)duration / durationMax);
            if (duration == 0)
            {
                Destruct();
            }
        }
    }

    protected virtual void Destruct()
    {
        ParticleManager.Instance.PlayParticle("BrokenParticle", this.transform.position);
        polygonCollider2D.enabled = false;
        gameObject.SetActive(false);
        AStar.TileGrid.Instance.Bake(spriteRenderer);
        Destroy(this);
    }

}
