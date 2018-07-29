using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempActiveSkil : MonoBehaviour {
    protected void DestroyAndDeactive()
    {
        Destroy(this);
        this.gameObject.SetActive(false);
    }

}

public class HandTrap : TempActiveSkil
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UtilityClass.CheckLayer(collision.gameObject.layer, 16))
        {
            Character character = collision.GetComponent<Character>();
        }
    }   
}
