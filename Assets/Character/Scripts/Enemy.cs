using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    
    #region UnityFunc
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RoomManager.Instance.DieMonster();
            gameObject.SetActive(false);
        }
    }
    #endregion
    #region Func
    public override void Die()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
