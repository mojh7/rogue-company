using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

}

public abstract class TouchItem : Item
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            Active();
    }

    protected abstract void Active();
}

public class Coin : TouchItem
{
    protected override void Active()
    {
    }
}
