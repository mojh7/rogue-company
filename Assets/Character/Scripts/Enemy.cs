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
        buffManager = GetComponent<BuffManager>();
        isKnockBack = false;
    }

    private void Update()
    {
        AutoAim();
        weaponManager.AttackButtonDown();
        renderer.sortingOrder = -Mathf.RoundToInt(transform.position.y);
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = -Mathf.Abs(scaleVector.x);
            transform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = Mathf.Abs(scaleVector.x);
            transform.localScale = scaleVector;
        }
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
        scaleVector = transform.localScale;
        hp = 5;

        // 0630 Enemy용 buffManager 초기화
        buffManager.Init();
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

    /// <summary>총알에서 전달된 정보로 공격 처리</summary>
    public override float Attacked(TransferBulletInfo transferredInfo)
    {
        if (State.ALIVE != pState)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        float damage = transferredInfo.damage;
        // 크리티컬 공격
        if(criticalCheck < transferredInfo.criticalChance)
        {
            damage *= 2f;
        }
        hp -= damage;
        StopCoroutine(CoroutineAttacked());
        StartCoroutine(CoroutineAttacked());
        if (hp <= 0)
            Die();
        return damage;
    }

    /// <summary>총알 외의 충돌로 인한 공격과 넉백 처리</summary>
    public float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        if (State.ALIVE != pState)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        // 크리티컬 공격
        if (criticalCheck < criticalChance)
        {
            damage *= 2f;
        }
        hp -= damage;

        if (knockBack > 0)
            isKnockBack = true;

        // 넉백 총알 방향 : 총알 이동 방향 or 몬스터-총알 방향 벡터
        rgbody.velocity = Vector3.zero;

        // bullet과 충돌 Object 위치 차이 기반의 넉백  
        if (positionBasedKnockBack)
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
        return damage;
    }

    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (State.ALIVE != pState)
            return;

        if (null == statusEffectInfo) return;
        // 독
        // 화상

        // 넉백 + 밀기, - 당기기
        if (0 != statusEffectInfo.knockBack)
        {
            isKnockBack = true;

            // 넉백 총알 방향 : 총알 이동 방향 or 몬스터-총알 방향 벡터
            rgbody.velocity = Vector3.zero;

            // bullet과 충돌 Object 위치 차이 기반의 넉백  
            if (statusEffectInfo.positionBasedKnockBack)
            {
                rgbody.AddForce(statusEffectInfo.knockBack * ((Vector2)transform.position - statusEffectInfo.bulletPos).normalized);
            }
            // bullet 방향 기반의 넉백
            else
            {
                rgbody.AddForce(statusEffectInfo.knockBack * statusEffectInfo.bulletDir);
            }

            StopCoroutine(KnockBackCheck());
            StartCoroutine(KnockBackCheck());
        }

        // 슬로우
        // 스턴
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
        directionDegree = directionVector.GetDegFromVector();
    }
    #endregion
}
