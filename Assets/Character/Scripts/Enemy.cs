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
    private void Awake()
    {
        rgbody = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (State.ALIVE != pState)
            return;
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector2 dir = collision.gameObject.GetComponent<Bullet>().GetDirVector();
            Attacked(dir);
            if (hp<=0)
                Die();
        }
    }


    #endregion
    #region Func
    public void Init(Sprite _sprite)
    {
        sprite = _sprite;
        pState = State.ALIVE;
        renderer.sprite = sprite;
        renderer.color = new Color(1, 1, 1);
        hp = 5;
    }
    protected override void Die()
    {
        pState = State.DIE;
        EnemyGenerator.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
        gameObject.SetActive(false);
    }
    protected override void Attacked(Vector2 _dir)
    {
        hp--;
        rgbody.AddForce(_dir*5);
        StopCoroutine(CoroutineAttacked());
        StartCoroutine(CoroutineAttacked());
    }
    IEnumerator CoroutineAttacked()
    {
        renderer.color = new Color(1, 0, 0);
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        renderer.color = new Color(1, 1, 1);
    }
    #endregion
}
