using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyTrapObj : RandomSpriteObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.DESTROYTRAPOBJ;

        polygonCollider2D.isTrigger = true;
        EraseShadow();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>())
        {
            collision.GetComponent<Character>().Attacked(Vector2.zero, transform.position, 1, 0);
            Destruct();
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
