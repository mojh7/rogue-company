using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempActiveSkil : MonoBehaviour {
    protected bool isAvailable;
    protected void DestroyAndDeactive()
    {
        Destroy(this);
        this.gameObject.SetActive(false);
    }
    private void EndAnimation()
    {
        DestroyAndDeactive();
    }
}

public class HandTrap : TempActiveSkil
{
    public void Init()
    {
        GetComponent<Animator>().SetTrigger("handsUp");
        isAvailable = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAvailable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, 16))
        {
            isAvailable = false;
            Character character = collision.GetComponent<Character>();
            character.Attacked(Vector2.zero, transform.position, 2, 0, 0);
        }
    }   
}
