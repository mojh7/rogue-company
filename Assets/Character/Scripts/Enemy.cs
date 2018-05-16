using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public new SpriteRenderer renderer;
    #region setter
    #endregion

    #region getter

    public override Vector3 GetDirVector()
    {
        throw new System.NotImplementedException();
    }

    public override float GetDirDegree()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region UnityFunc
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (State.ALIVE != pState)
            return;
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Die();
            RoomManager.Instance.DieMonster();
            gameObject.SetActive(false);
        }
    }


    #endregion
    #region Func
    public void Init(Sprite _sprite)
    {
        sprite = _sprite;
        pState = State.ALIVE;
        renderer.sprite = sprite;   
    }
    public override void Die()
    {
        pState = State.DIE;
    }
    #endregion
}
