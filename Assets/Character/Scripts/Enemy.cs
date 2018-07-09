using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StatusConstant
{
    public float value;         // 각 상태이상 별로 세기, 값
    public float effectiveTime; // 효과 유지 시간
    public int overlapCountMax; // 중첩 횟수 최대치
    public StatusConstant(float value, float effectiveTime, int overlapCountMax)
    {
        this.value = value;
        this.effectiveTime = effectiveTime;
        this.overlapCountMax = overlapCountMax;
    }
}

public static class StatusConstants
{
    public static float damageTerm = 0.1f;
    public static StatusConstant poisonInfo = new StatusConstant(0.05f, 3f, 3);
    public static StatusConstant burnInfo   = new StatusConstant(0.05f, 3f, 3);
    public static StatusConstant nagInfo    = new StatusConstant(0.5f, 4f, 2);
    public static StatusConstant delayStateInfo = new StatusConstant(0.5f, 3f, 3);
}

public class Enemy : Character
{
    public bool isKnockBack;
    EnemyData enemyData;

    private bool isPoisoning;
    private int PoisonCount;
    private bool isBurning;
    private int burnCount;
    private bool isNagging;
    private int nagCount;
    public bool isDelayingState;
    private int delayStateCount;

    private Coroutine poisonCoroutine;
    private Coroutine burnCoroutine;
    private Coroutine nagCoroutine;
    private Coroutine delayStateCoroutine;

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
        if (CharacterInfo.State.ALIVE != pState)
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
    //    pState = CharacterInfo.State.ALIVE;
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
        pState = CharacterInfo.State.ALIVE;
        renderer.sprite = sprite;
        renderer.color = new Color(1, 1, 1);
        scaleVector = transform.localScale;
        hp = 5;

        isPoisoning = false;
        PoisonCount = 0;
        isBurning = false;
        burnCount = 0;
        isNagging = false;
        nagCount = 0;
        isDelayingState = false;
        delayStateCount = 0;

        // 0630 Enemy용 buffManager 초기화
        buffManager.Init();
        buffManager.SetOwner(this);
        // 0526 임시용
        weaponManager = GetComponentInChildren<WeaponManager>();
        weaponManager.Init(this, CharacterInfo.OwnerType.Enemy);
    }
    protected override void Die()
    {
        pState = CharacterInfo.State.DIE;
        // 실행 중인 코루틴이 있으면 코루틴 멈춤
        if (null != poisonCoroutine)
        {
            StopCoroutine(poisonCoroutine);
        }
        if (null != burnCoroutine)
        {
            StopCoroutine(burnCoroutine);
        }
        if (null != nagCoroutine)
        {
            StopCoroutine(nagCoroutine);
        }
        if (null != delayStateCoroutine)
        {
            StopCoroutine(delayStateCoroutine);
        }
        
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
        if (CharacterInfo.State.ALIVE != pState)
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
        if (CharacterInfo.State.ALIVE != pState)
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

    public override void ApplyItemEffect(CharacterTargetEffect itemUseEffect)
    {

    }

    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return;

        if (null == statusEffectInfo) return;
        // 독
        if(true == statusEffectInfo.canPoison)
        {
            Poison();
        }

        // 화상
        if(true == statusEffectInfo.canBurn)
        {
            Burn();
        }

        // 넉백 : + 밀기, - 당기기
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

        if (true == statusEffectInfo.canNag)
        {
            Nag();
        }

        if (true == statusEffectInfo.canDelayState)
        {
            DelayState();
        }
    }
    public void Poison()
    {
        if (PoisonCount >= StatusConstants.poisonInfo.overlapCountMax)
            return;
        if(false == isPoisoning)
        {
            poisonCoroutine = StartCoroutine(PoisonCoroutine());
        }
        else
        {

        }
        PoisonCount += 1;
    }
    public void Burn()
    {
        if (burnCount >= StatusConstants.burnInfo.overlapCountMax)
            return;
        burnCount += 1;
    }

    // 이동 translate, velocity, addForce ??
    public override void Nag()
    { 
        // 처음 거는거
        if(false == isNagging)
        {
            NagCoroutine();
        }
        else
        {
            if(nagCount >= StatusConstants.nagInfo.overlapCountMax)
            {

            }
        }
            
        {

        }
    }

    //
    public override void DelayState()
    {
        if(false == isDelayingState)
        {
            delayStateCoroutine = StartCoroutine(DelayStateCoroutine());
            delayStateCount += 1;
            isDelayingState = true;
        }
        else
        {
            if (delayStateCount >= StatusConstants.delayStateInfo.overlapCountMax)
                return;
            else
            {
                delayStateCount += 1;
            }
        }
    }
    IEnumerator PoisonCoroutine()
    {
        
        yield return YieldInstructionCache.WaitForSeconds(StatusConstants.damageTerm);
    }

    IEnumerator BurnCoroutine()
    {

        yield return YieldInstructionCache.WaitForSeconds(StatusConstants.damageTerm);
    }

    IEnumerator NagCoroutine()
    {
        yield return YieldInstructionCache.WaitForSeconds(StatusConstants.nagInfo.value);
    }

    IEnumerator DelayStateCoroutine()
    {
        int delayStateCount = this. delayStateCount;
        while (true)
        {
            CharacterTargetEffect characterTargetEffect = new CharacterTargetEffect();
            WeaponTargetEffect weaponTargetEffect = new WeaponTargetEffect();
            characterTargetEffect.moveSpeedIncrease = -StatusConstants.delayStateInfo.value;
            weaponTargetEffect.bulletSpeedIncrease = -StatusConstants.delayStateInfo.value;
            buffManager.RegisterItemEffect(characterTargetEffect, StatusConstants.delayStateInfo.effectiveTime);
            buffManager.RegisterItemEffect(weaponTargetEffect, StatusConstants.delayStateInfo.effectiveTime);
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.delayStateInfo.effectiveTime);
            if (delayStateCount == 1) break;
        }
        isDelayingState = false;
        delayStateCount = 0;
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
