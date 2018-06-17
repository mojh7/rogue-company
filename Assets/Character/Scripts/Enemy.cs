using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    public bool isKnockBack;

    EnemyData enemyData;
    #region setter
    #endregion

    #region getter
    public float GetHP() { return hp; }
    #endregion

    #region UnityFunc
    private void Awake()
    {
        rgbody = GetComponent<Rigidbody2D>();
        isKnockBack = false;
    }

    private void Update()
    {
        AutoAim();
        weaponManager.AttackButtonDown();
        renderer.sortingOrder = -(int)transform.position.y;
    }
    /*
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
    }*/


    #endregion
    #region Func
    //0603 이유성 적 데이터로 적만들기 (애니메이션 아직 보류)
    //public void Init(EnemyData enemyData)
    //{
    //    pState = State.ALIVE;
    //    hp = enemyData.HP;
    //    moveSpeed = enemyData.Speed;
    //    animator = enemyData.Animator;
    //    weaponManager = GetComponentInChildren<WeaponManager>();
    //    weaponManager.Init(this, OwnerType.Enemy);
    //    weaponManager.EquipWeapon(enemyData.WeaponInfo);
    //}
    public void Init(Sprite _sprite)
    {
        sprite = _sprite;
        pState = State.ALIVE;
        renderer.sprite = sprite;
        renderer.color = new Color(1, 1, 1);
        hp = 5;

        // 0526 임시용
        weaponManager = GetComponentInChildren<WeaponManager>();
        weaponManager.Init(this, OwnerType.Enemy);
    }
    protected override void Die()
    {
        pState = State.DIE;
        EnemyManager.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
        gameObject.SetActive(false);
        DropItem();
    }
    protected void DropItem()
    {
        GameObject coin = new GameObject();
        coin.AddComponent<SpriteRenderer>().sprite = ItemManager.Instance.coinSprite;
        coin.AddComponent<Coin>();
        ItemManager.Instance.CreateItem(coin.GetComponent<Coin>(), transform.position);
    }

    public override void Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalRate, bool positionBasedKnockBack = false)
    {
        //Debug.Log(damage + ", " + knockBack + ", " + criticalRate);

        if (State.ALIVE != pState)
            return;

        float criticalCheck = Random.Range(0f, 1f);
        /*
        if(criticalCheck < criticalRate)
        {
            Debug.Log("critical Attack");
        }
        else
        {
            Debug.Log("normal Attack");
        }*/

        hp -= damage;
        if (knockBack > 0)
            isKnockBack = true;

        // 넉백 총알 방향 : 총알 이동 방향 or 몬스터-총알 방향 벡터
        rgbody.velocity = Vector3.zero;
        
        // bullet과 충돌 Object 위치 차이 기반의 넉백  
        if(positionBasedKnockBack)
        {
            rgbody.AddForce(knockBack * ((Vector2)transform.position - bulletPos).normalized);
        }
        // bullet 방향 기반의 넉백
        else
        {
            rgbody.AddForce(knockBack * _dir);
        }

        StopCoroutine(KnockBackCheck());
        StartCoroutine(KnockBackCheck());
        StopCoroutine(CoroutineAttacked());
        StartCoroutine(CoroutineAttacked());
        if (hp <= 0)
            Die();
    }
    IEnumerator CoroutineAttacked()
    {
        renderer.color = new Color(1, 0, 0);
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        renderer.color = new Color(1, 1, 1);
    }

    IEnumerator KnockBackCheck()
    {
        
        while(true)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (Vector2.zero != rgbody.velocity && rgbody.velocity.magnitude < 1f)
            {
                isKnockBack = false;
            }
        }
        
        /*yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime * 25);
        isKnockBack = false;*/
    }

    // 0526 땜빵

    public void AutoAim()
    {
        directionVector = (PlayerManager.Instance.GetPlayer().GetPosition() - transform.position).normalized;
            //controller.GetRecenteNormalInputVector();
        directionDegree = directionVector.GetDegFromVector();
    }
    #endregion
}
